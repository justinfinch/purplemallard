using Microsoft.AspNetCore.Http;

namespace PurpleMallard.Bff.Endpoints;

public interface IBffEndpoint
{
    /// <summary>
    /// Process a request
    /// </summary>
    /// <returns></returns>
    Task ProcessRequestAsync(HttpContext context, CancellationToken cancellationToken = default);
}
