using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurpleMallard.Bff.Config;
using PurpleMallard.Bff.Otel;

namespace PurpleMallard.Bff.Endpoints;

internal class LoginEndpoint(
    IReturnUrlValidator returnUrlValidator,
    ILogger<LoginEndpoint> logger) : ILoginEndpoint
{
     public async Task ProcessRequestAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Processing login request");

        var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            if (!returnUrlValidator.IsValidAsync(returnUrl))
            {
                logger.InvalidReturnUrl(LogLevel.Information, returnUrl.Sanitize());
                context.ReturnHttpProblem("Invalid return url", (Constants.RequestParameters.ReturnUrl, [$"ReturnUrl '{returnUrl}' was invalid"]));
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = context.Request.PathBase.HasValue
                ? context.Request.PathBase
                : "/";
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        logger.LogDebug("Login endpoint triggering Challenge with returnUrl {returnUrl}", returnUrl.Sanitize());

        await context.ChallengeAsync(props);
    }
}
