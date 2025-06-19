using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using PurpleMallard.Products.Api.Features.ProductAssistant.Agents;

namespace PurpleMallard.Products.Api.Features.ProductAssistant;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductAssistant(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ProductAssistantOptions>()
                        .Bind(configuration.GetSection(ProductAssistantOptions.SectionName))
                        .ValidateDataAnnotations()
                        .ValidateOnStart();

        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<ProductAssistantOptions>>().Value;

        services.AddAzureOpenAIChatCompletion(
            deploymentName: options.AzureOpenAI.ChatDeploymentName,
            apiKey: options.AzureOpenAI.ApiKey,
            endpoint: options.AzureOpenAI.Endpoint,
            modelId: options.AzureOpenAI.ModelId,
            serviceId: Constants.ProductAssistantServiceKeys.ProductAssistantLlm
        );

        services.AddKeyedTransient(Constants.ProductAssistantServiceKeys.ProductAssistantKernel, (sp, key) =>
        {
            return new Kernel(sp);
        });

        services.AddScoped<IProductAssistantAgent, ProductAssistantAgent>();

        return services;
    }
}
