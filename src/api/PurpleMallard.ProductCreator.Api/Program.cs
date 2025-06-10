using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy => 
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") // Allow both localhost and IP address
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Allow credentials (cookies, auth headers)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowReactDev");
}

app.UseHttpsRedirection();
app.UseFastEndpoints();

app.Run();
