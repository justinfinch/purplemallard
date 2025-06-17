using FastEndpoints;
using FastEndpoints.Swagger; // Add Swagger namespace
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddFastEndpoints();

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "PurpleMallard",
        configureOptions: options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                // You can specify the audience if you have a specific API client in Keycloak
                // options.Audience = "api-client";
            };
        });

// Configure authorization with a default policy that requires authentication
builder.Services.AddAuthorization(options =>
{
    // Add a default policy that requires authentication
    options.AddPolicy("Default", policy => policy.RequireAuthenticatedUser());
    options.DefaultPolicy = options.GetPolicy("Default")!;
});

builder.Services.SwaggerDocument(o => {
    o.DocumentSettings = s => {
        s.Title = "Purple Mallard Product Creator API";
        s.Version = "v1";
        s.Description = "API for managing product creation in the Purple Mallard system";
    };
    o.EnableJWTBearerAuth = true; // Enable JWT authentication in Swagger
    o.ShortSchemaNames = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development
    app.UseSwaggerGen();
    
    // Add a redirect from root to swagger
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Use FastEndpoints - individual endpoints will specify authorization requirements
app.UseFastEndpoints();

app.Run();
