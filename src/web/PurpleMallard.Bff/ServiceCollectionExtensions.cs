using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PurpleMallard.Bff.Endpoints;

namespace PurpleMallard.Bff;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBff(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();

        services.AddAuthorization();

        services.AddTransient<IReturnUrlValidator, LocalUrlReturnUrlValidator>();

        services.AddTransient<ILoginEndpoint, LoginEndpoint>();
        services.AddTransient<IUserEndpoint, UserEndpoint>();

        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddServiceDiscoveryDestinationResolver(); // Add Aspire service discovery

        return services;
    }
}
