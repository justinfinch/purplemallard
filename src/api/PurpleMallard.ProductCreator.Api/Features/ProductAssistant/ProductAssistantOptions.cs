using System;
using System.ComponentModel.DataAnnotations;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

public sealed class ProductAssistantOptions
{
    public const string SectionName = "ProductAssistant";

    [Required]
    public required AzureOpenAIOptions AssistantLlm { get; set; }

}


public sealed class AzureOpenAIOptions
{
    [Required]
    public string ChatDeploymentName { get; set; } = string.Empty;

    [Required]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    public string? ModelId { get; set; } = null;
}