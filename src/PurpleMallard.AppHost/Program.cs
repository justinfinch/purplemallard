var builder = DistributedApplication.CreateBuilder(args);

var kcUsername = builder.AddParameter("admin", value: "admin");
var kcPassword = builder.AddParameter("password", secret: true, value: "password");

var keycloak = builder.AddKeycloak("keycloak", 8080, kcUsername, kcPassword)
    .WithDataVolume()
    .WithRealmImport("./realms")
    .WithExternalHttpEndpoints();

// Add the ProductCreator API
var productCreatorApi = builder.AddProject<Projects.PurpleMallard_ProductCreator_Api>("productcreatorapi")
    .WithReference(keycloak)
    .WaitFor(keycloak); 

// Add the SPA Host (with YARP Reverse Proxy)
var spaHost = builder.AddProject<Projects.PurpleMallard_Spa_Host>("spahost")
    .WithReference(productCreatorApi)
    .WithReference(keycloak)
    .WaitFor(productCreatorApi)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

builder.Build().Run();
