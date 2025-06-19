using FastEndpoints;
using FastEndpoints.Swagger; // Add Swagger namespace
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PurpleMallard.Products.Api.Features.ProductAssistant;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add Redis distributed caching
builder.AddRedisDistributedCache("cache");

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
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            options.Audience = "products-api"; // Set the audience for the API
        });

// Configure authorization with a default policy that requires authentication
builder.Services.AddAuthorization(options =>
{
    // Add a default policy that requires authentication
    options.AddPolicy("Default", policy => policy.RequireAuthenticatedUser());
    options.DefaultPolicy = options.GetPolicy("Default")!;
});

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Purple Mallard Products API";
        s.Version = "v1";
        s.Description = "API for managing products in the Purple Mallard system";
    };
    o.EnableJWTBearerAuth = true; // Enable JWT authentication in Swagger
    o.ShortSchemaNames = true;
});

// Add our module services
builder.Services.AddProductAssistant(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Use FastEndpoints - individual endpoints will specify authorization requirements
app.UseFastEndpoints();

app.Run();
