using System.Net.Http.Headers;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurpleMallard.Bff.AccessTokenManagement;
using PurpleMallard.Bff.Config;
using PurpleMallard.Bff.Otel;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;

namespace PurpleMallard.Bff.Yarp;

/// <summary>
/// Adds an access token to outgoing requests
/// </summary>
internal class AccessTokenRequestTransform(
    IOptions<BffOptions> options,
    ILogger<AccessTokenRequestTransform> logger) : RequestTransform
{
    /// <inheritdoc />
    public override async ValueTask ApplyAsync(RequestTransformContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint == null)
        {
            throw new InvalidOperationException("endpoint not found");
        }

        UserTokenRequestParameters? userAccessTokenParameters = null;

        // Get the metadata
        var metadata = GetBffMetadataFromYarp(endpoint)
            ?? throw new InvalidOperationException("API endpoint is missing BFF metadata");

        if (metadata.UserTokenRequestParameters != null)
        {
            userAccessTokenParameters = metadata.UserTokenRequestParameters;
        }

        if (context.HttpContext.RequestServices.GetRequiredService(metadata.AccessTokenRetriever)
            is not IAccessTokenRetriever accessTokenRetriever)
        {
            throw new InvalidOperationException("TokenRetriever is not an IAccessTokenRetriever");
        }

        var accessTokenContext = new AccessTokenRetrievalContext()
        {
            HttpContext = context.HttpContext,
            Metadata = metadata,
            UserTokenRequestParameters = userAccessTokenParameters,
            ApiAddress = new Uri(context.DestinationPrefix),
            LocalPath = context.HttpContext.Request.Path
        };
        var result = await accessTokenRetriever.GetAccessTokenAsync(accessTokenContext);

        switch (result)
        {
            case BearerTokenResult bearerToken:
                ApplyBearerToken(context, bearerToken);
                break;
            case AccessTokenRetrievalError tokenError:

                if (ShouldSignOutUser(tokenError, metadata))
                {
                    // see if we need to sign out
                    var authenticationSchemeProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                    // get rid of local cookie first
                    var signInScheme = await authenticationSchemeProvider.GetDefaultSignInSchemeAsync();
                    await context.HttpContext.SignOutAsync(signInScheme?.Name);
                }

                ApplyError(context, tokenError, metadata.TokenType);
                break;
            case NoAccessTokenResult:
                break;
        }
    }

    private bool ShouldSignOutUser(AccessTokenRetrievalError tokenError, BffRemoteApiEndpointMetadata metadata)
    {
        if (metadata.TokenType == RequiredTokenType.User ||
            metadata.TokenType == RequiredTokenType.UserOrClient)
        {
            LogMessages.UserSessionRevoked(logger, tokenError.Error);
            return true;
        }

        return false;
    }

    private static BffRemoteApiEndpointMetadata? GetBffMetadataFromYarp(Endpoint endpoint)
    {
        var yarp = endpoint.Metadata.GetMetadata<RouteModel>();
        if (yarp == null)
        {
            return null;
        }

        RequiredTokenType? requiredTokenType = null;
        if (Enum.TryParse<RequiredTokenType>(yarp.Config.Metadata?.GetValueOrDefault(Constants.Yarp.TokenTypeMetadata), true, out var type))
        {
            requiredTokenType = type;
        }

        return new BffRemoteApiEndpointMetadata()
        {
            TokenType = requiredTokenType
        };
    }

    private void ApplyError(RequestTransformContext context, AccessTokenRetrievalError tokenError, RequiredTokenType? tokenType)
    {
        // short circuit forwarder and return 401
        context.HttpContext.Response.StatusCode = 401;

        logger.AccessTokenMissing(LogLevel.Warning,
            tokenType?.ToString() ?? "Unknown token type",
            context.HttpContext.Request.Path.Sanitize(),
            tokenError.Error);
    }

    private void ApplyBearerToken(RequestTransformContext context, BearerTokenResult token) => context.ProxyRequest.Headers.Authorization =
            new AuthenticationHeaderValue(OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer, token.AccessToken.ToString());
}
