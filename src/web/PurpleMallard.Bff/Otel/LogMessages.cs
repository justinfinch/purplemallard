using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PurpleMallard.Bff.Otel;

internal static partial class LogMessages
{

    [LoggerMessage(
        $"Invalid return url {{{OTelParameters.Url}}}.")]
    public static partial void InvalidReturnUrl(this ILogger logger, LogLevel logLevel, string url);

    public static string Sanitize(this string toSanitize) => toSanitize.ReplaceLineEndings(string.Empty);

    public static string Sanitize(this PathString toSanitize) => toSanitize.ToString().ReplaceLineEndings(string.Empty);
}
