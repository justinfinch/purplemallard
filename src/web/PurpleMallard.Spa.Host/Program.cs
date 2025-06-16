
using PurpleMallard.Bff;


var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddBff()
    .WithDefaultOpenIdConnectOptions(options =>
    {
        options.Authority = "https://tbd";
        options.ClientId = "interactive.confidential";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.MapInboundClaims = false;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        // Add this scope if you want to receive refresh tokens
        options.Scope.Add("offline_access");
    })
    .WithDefaultCookieOptions(options =>
    {
        // Because we use an identity server that's configured on a different site
        // (domain.com vs localhost), we need to configure the SameSite property to Lax.
        // Setting it to Strict would cause the authentication cookie not to be sent after logging in.
        // The user would have to refresh the page to get the cookie.
        // Recommendation: Set it to 'strict' if your IDP is on the same site as your BFF.
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapBffManagementEndpoints();

app.MapFallbackToFile("index.html");
app.Run();
