using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace PurpleMallard.Bff.Config;

public sealed class BffOptions
{
    /// <summary>
    /// Base path for management endpoints. Defaults to "/bff".
    /// </summary>
    public PathString BffBasePath { get; set; } = "/bff";

    /// <summary>
    /// Login endpoint
    /// </summary>
    public PathString LoginPath => BffBasePath.Add(Constants.BffEndpoints.Login);

    /// <summary>
    /// User endpoint
    /// </summary>
    public PathString UserPath => BffBasePath.Add(Constants.BffEndpoints.User);

    /// <summary>
    /// Logout endpoint
    /// </summary>
    public PathString LogoutPath => BffBasePath.Add(Constants.BffEndpoints.Logout);
}
