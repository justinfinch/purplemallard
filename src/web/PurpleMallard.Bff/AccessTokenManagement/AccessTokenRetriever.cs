using System;
using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PurpleMallard.Bff.AccessTokenManagement;

internal class AccessTokenRetriever() : IAccessTokenRetriever
{
    /// <inheritdoc />
    public async Task<AccessTokenResult> GetAccessTokenAsync(AccessTokenRetrievalContext context, CancellationToken cancellationToken = default)
    {
        if (context.Metadata.TokenType.HasValue)
        {
            return await GetManagedAccessToken(context.HttpContext,
                requiredTokenType: context.Metadata.TokenType.Value,
                context.UserTokenRequestParameters,
                cancellationToken: cancellationToken);
        }
        else
        {
            return new NoAccessTokenResult();
        }
    }

    private async Task<AccessTokenResult> GetManagedAccessToken(
        HttpContext context,
        RequiredTokenType requiredTokenType,
        UserTokenRequestParameters? userAccessTokenRequestParameters = null,
        CancellationToken cancellationToken = default)
    {
        if (requiredTokenType == RequiredTokenType.None)
        {
            return new NoAccessTokenResult();
        }

        RequiredTokenType[] shouldGetUserToken = [
            RequiredTokenType.User,
            RequiredTokenType.UserOrNone,
            RequiredTokenType.UserOrClient
        ];

        if (shouldGetUserToken.Contains(requiredTokenType))
        {
            var userTokenResult = await
                context.GetUserAccessTokenAsync(userAccessTokenRequestParameters, cancellationToken);

            if (userTokenResult.WasSuccessful(out var userToken, out var userTokenFailure))
            {
                return new BearerTokenResult
                {
                    // Should we append the type here?
                    AccessToken = AccessToken.Parse(userToken.AccessToken.ToString())
                };
            }

            if (requiredTokenType == RequiredTokenType.User)
            {
                return new AccessTokenRetrievalError
                {
                    Error = userTokenFailure.Error,
                    ErrorDescription = userTokenFailure.ErrorDescription
                };
            }
            
            if (requiredTokenType == RequiredTokenType.UserOrNone)
            {
                return new NoAccessTokenResult();
            }
        }

        var clientTokenResult = await context.GetClientAccessTokenAsync(userAccessTokenRequestParameters, cancellationToken);
        if (clientTokenResult.WasSuccessful(out var clientToken, out var clientTokenFailure))
        {
            return new BearerTokenResult
            {
                // Should we append the type here?

                AccessToken = AccessToken.Parse(clientToken.AccessToken.ToString())
            };
        }

        return new AccessTokenRetrievalError
        {
            Error = clientTokenFailure.Error,
            ErrorDescription = clientTokenFailure.ErrorDescription
        };
    }
}
