var builder = DistributedApplication.CreateBuilder(args);

// Add the ProductCreator API
var productCreatorApi = builder.AddProject<Projects.PurpleMallard_ProductCreator_Api>("productcreatorapi");

// Add the SPA Host (with YARP Reverse Proxy)
var spaHost = builder.AddProject<Projects.PurpleMallard_Spa_Host>("spahost")
    .WithReference(productCreatorApi)
    .WaitFor(productCreatorApi)
    .WithExternalHttpEndpoints();;

builder.Build().Run();
