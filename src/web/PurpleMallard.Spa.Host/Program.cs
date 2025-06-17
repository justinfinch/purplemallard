
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PurpleMallard.Bff;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;
var cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme;

builder.Services.AddAuthentication(options => 
    {
        options.DefaultScheme = cookieScheme;
        options.DefaultChallengeScheme = oidcScheme;
        options.DefaultSignOutScheme = oidcScheme;
    })
    .AddKeycloakOpenIdConnect("keycloak", realm: "PurpleMallard", oidcScheme, options =>
    {
        options.ClientId = "PurpleMallard_Spa_Host";
        options.ClientSecret = "e0SbiGMUiDg71KyKjN8jzcN31HVJqFVg";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
        options.SaveTokens = true;
        options.SignInScheme = cookieScheme;

        options.Scope.Add("purple-mallard-spa:all");
    })
    .AddCookie(cookieScheme);

builder.Services.AddBff(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapBffManagementEndpoints();

app.MapReverseProxy();

app.MapFallbackToFile("index.html");
app.Run();
