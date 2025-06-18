using System;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using PurpleMallard.ProductCreator.Api.Features.ProductAssistant.Agents;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

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
            deploymentName: options.AssistantLlm.ChatDeploymentName,
            apiKey: options.AssistantLlm.ApiKey,
            endpoint: options.AssistantLlm.Endpoint,
            modelId: options.AssistantLlm.ModelId,
            serviceId: Constants.ProductAssistantServiceKeys.ProductAssistantLlm
        );

        services.AddKeyedTransient(Constants.ProductAssistantServiceKeys.ProductAssistantKernel, (sp, key) =>
        {
            return new Kernel(sp);
        });

        services.AddScoped<IProductAssistantConversationService, ProductAssistantConversationService>();

        return services;
    }
}
