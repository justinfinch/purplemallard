
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PurpleMallard.Bff;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

builder.Services.AddAuthentication(oidcScheme)
    .AddKeycloakOpenIdConnect("keycloak", realm: "PurpleMallard", oidcScheme, options =>
    {
        options.ClientId = "PurpleMallard_Spa_Host";
        options.ClientSecret = "8mfUcAJj8YIylWWXWs6EYL025M6b4LyH";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
        options.SaveTokens = true;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();

builder.Services.AddBff();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapBffManagementEndpoints();

app.MapFallbackToFile("index.html");
app.Run();
