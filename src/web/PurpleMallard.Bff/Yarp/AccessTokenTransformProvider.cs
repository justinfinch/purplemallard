using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PurpleMallard.Bff.AccessTokenManagement;
using PurpleMallard.Bff.Config;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace PurpleMallard.Bff.Yarp;

/// <summary>
/// Transform provider to attach an access token to forwarded calls
/// </summary>
internal class AccessTokenTransformProvider(IOptions<BffOptions> options,
    ILoggerFactory loggerFactory) : ITransformProvider
{
    private readonly BffOptions _options = options.Value;

    /// <inheritdoc />
    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    /// <inheritdoc />
    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    private static bool GetMetadataValue(TransformBuilderContext transformBuildContext, string metadataName, [NotNullWhen(true)] out string? metadata)
    {
        var routeValue = transformBuildContext.Route.Metadata?.GetValueOrDefault(metadataName);
        var clusterValue =
            transformBuildContext.Cluster?.Metadata?.GetValueOrDefault(metadataName);

        // no metadata
        if (string.IsNullOrEmpty(routeValue) && string.IsNullOrEmpty(clusterValue))
        {
            metadata = null;
            return false;
        }

        var values = new HashSet<string>();
        if (!string.IsNullOrEmpty(routeValue))
        {
            values.Add(routeValue);
        }

        if (!string.IsNullOrEmpty(clusterValue))
        {
            values.Add(clusterValue);
        }

        if (values.Count > 1)
        {
            throw new ArgumentException(
                $"Mismatching {metadataName} route and cluster metadata values found");
        }

        metadata = values.First();
        return true;
    }

    /// <inheritdoc />
    public void Apply(TransformBuilderContext transformBuildContext)
    {
        if (GetMetadataValue(transformBuildContext, Constants.Yarp.TokenTypeMetadata, out var tokenTypeMetadata))
        {
            if (!Enum.TryParse<RequiredTokenType>(tokenTypeMetadata, true, out _))
            {
                throw new ArgumentException("Invalid value for PurpleMallard.Bff.Yarp.TokenType metadata");
            }
        }
        else
        {
            return;
        }

        transformBuildContext.AddRequestTransform(async transformContext =>
        {
            var accessTokenTransform = new AccessTokenRequestTransform(
                Options.Create(_options),
                loggerFactory.CreateLogger<AccessTokenRequestTransform>());

            await accessTokenTransform.ApplyAsync(transformContext);
        });
    }
}
