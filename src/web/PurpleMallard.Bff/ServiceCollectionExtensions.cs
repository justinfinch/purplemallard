using System;
using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PurpleMallard.Bff.AccessTokenManagement;
using PurpleMallard.Bff.Endpoints;
using PurpleMallard.Bff.Yarp;

namespace PurpleMallard.Bff;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBff(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();

        services.AddOpenIdConnectAccessTokenManagement();
        services.TryAddSingleton<IAccessTokenRetriever, AccessTokenRetriever>();

        services.AddAuthorization();

        services.AddTransient<IReturnUrlValidator, LocalUrlReturnUrlValidator>();

        services.AddTransient<ILoginEndpoint, LoginEndpoint>();
        services.AddTransient<IUserEndpoint, UserEndpoint>();

        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddServiceDiscoveryDestinationResolver() // Add Aspire service discovery
            .AddTransforms<AccessTokenTransformProvider>();

        return services;
    }
}
