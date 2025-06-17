using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PurpleMallard.Bff.Otel;

internal static partial class LogMessages
{

    [LoggerMessage(
        $"Invalid return url {{{OTelParameters.Url}}}.")]
    public static partial void InvalidReturnUrl(this ILogger logger, LogLevel logLevel, string url);

    [LoggerMessage(
        message: $"Access token is missing. token type: '{{{OTelParameters.TokenType}}}', local path: '{{{OTelParameters.LocalPath}}}', detail: '{{{OTelParameters.Detail}}}'")]
    public static partial void AccessTokenMissing(this ILogger logger, LogLevel logLevel, string tokenType, string localPath, string detail);

    [LoggerMessage(
        level: LogLevel.Warning,
        message: $"Failed to request new User Access Token due to: {{{OTelParameters.Error}}}. This likely means that the user's refresh token is expired or revoked. The user's session will be ended, which will force the user to log in.")]
    public static partial void UserSessionRevoked(this ILogger logger, string error);

    public static string Sanitize(this string toSanitize) => toSanitize.ReplaceLineEndings(string.Empty);

    public static string Sanitize(this PathString toSanitize) => toSanitize.ToString().ReplaceLineEndings(string.Empty);
}
