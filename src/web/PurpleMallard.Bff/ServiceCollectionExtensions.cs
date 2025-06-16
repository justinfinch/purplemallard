using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using PurpleMallard.Bff.Endpoints;

namespace PurpleMallard.Bff;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBff(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddTransient<IReturnUrlValidator, LocalUrlReturnUrlValidator>();

        services.AddTransient<ILoginEndpoint, LoginEndpoint>();
        services.AddTransient<IUserEndpoint, UserEndpoint>();

        return services;
    }
}
