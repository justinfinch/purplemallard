using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PurpleMallard.Bff.Config;
using PurpleMallard.Bff.Endpoints;

namespace PurpleMallard.Bff;

public static class EndpointRouteBuilderExtensions
{
    private static Task ProcessWith<T>(HttpContext context)
        where T : IBffEndpoint
    {
        var service = context.RequestServices.GetRequiredService<T>();
        return service.ProcessRequestAsync(context);
    }

    public static void MapBffManagementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapBffLoginEndpoint();
        endpoints.MapBffUserEndpoint();
    }

    /// <summary>
    /// Adds the login endpoint
    /// </summary>
    /// <param name="endpoints"></param>
    public static void MapBffLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
       var options = endpoints.ServiceProvider.GetRequiredService<IOptions<BffOptions>>().Value;
        
        endpoints.MapGet(options.LoginPath.Value!, ProcessWith<ILoginEndpoint>)
            .AllowAnonymous();
    }

    /// <summary>
    /// Adds the user endpoint
    /// </summary>
    /// <param name="endpoints"></param>
    public static void MapBffUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
       var options = endpoints.ServiceProvider.GetRequiredService<IOptions<BffOptions>>().Value;

        endpoints.MapGet(options.UserPath.Value!, ProcessWith<IUserEndpoint>)
            .AllowAnonymous();
    }
}
