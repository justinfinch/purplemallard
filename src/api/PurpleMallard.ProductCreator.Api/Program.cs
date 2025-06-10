using FastEndpoints;
using FastEndpoints.Swagger; // Add Swagger namespace

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o => {
    o.DocumentSettings = s => {
        s.Title = "Purple Mallard Product Creator API";
        s.Version = "v1";
        s.Description = "API for managing product creation in the Purple Mallard system";
    };
    o.EnableJWTBearerAuth = false; // Change to true if you implement JWT authentication
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
app.UseFastEndpoints();

app.Run();
