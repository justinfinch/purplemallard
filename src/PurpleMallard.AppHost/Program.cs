var builder = DistributedApplication.CreateBuilder(args);

var kcUsername = builder.AddParameter("admin", value: "admin");
var kcPassword = builder.AddParameter("password", secret: true, value: "password");

var keycloak = builder.AddKeycloak("keycloak", 8080, kcUsername, kcPassword)
    .WithDataVolume()
    .WithRealmImport("./realms")
    .WithExternalHttpEndpoints();

// Add Redis for distributed caching
var redis = builder.AddRedis("cache")
    .WithDataVolume()
    .WithRedisCommander();

// Add the Products API
var productsApi = builder.AddProject<Projects.PurpleMallard_Products_Api>("productsapi")
    .WithReference(keycloak)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(keycloak);

// Add the SPA Host (with YARP Reverse Proxy)
var spaHost = builder.AddProject<Projects.PurpleMallard_Spa_Host>("spahost")
    .WithReference(productsApi)
    .WithReference(keycloak)
    .WithReference(redis)
    .WaitFor(productsApi)
    .WaitFor(keycloak)
    .WaitFor(redis)
    .WithExternalHttpEndpoints();

builder.Build().Run();
