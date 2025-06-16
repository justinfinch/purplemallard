using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using PurpleMallard.Bff.Endpoints;

namespace PurpleMallard.Bff;

public static class ServiceCollectionExtensions
{
    public static BffBuilder AddBff(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddTransient<IReturnUrlValidator, LocalUrlReturnUrlValidator>();

        services.AddTransient<ILoginEndpoint, LoginEndpoint>();
        services.AddTransient<IUserEndpoint, UserEndpoint>();

        // wrap ASP.NET Core
        services.AddAuthentication();

        return new BffBuilder(services);
    }
}
