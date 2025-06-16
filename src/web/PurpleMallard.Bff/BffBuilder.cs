using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PurpleMallard.Bff.Config;

namespace PurpleMallard.Bff;

public sealed class BffBuilder(IServiceCollection services)
{
    internal IConfiguration? Configuration { get; private set; }

    /// <summary>
    /// The service collection
    /// </summary>
    public IServiceCollection Services { get; } = services;


    public BffBuilder WithDefaultOpenIdConnectOptions(Action<OpenIdConnectOptions> oidc)
    {
        Services.Configure<BffOptions>(bffOptions => bffOptions.ConfigureOpenIdConnectDefaults += oidc);
        return this;
    }

    public BffBuilder WithDefaultCookieOptions(Action<CookieAuthenticationOptions> cookie)
    {
        Services.Configure<BffOptions>(bffOptions => bffOptions.ConfigureCookieDefaults += cookie);
        return this;
    }
}
