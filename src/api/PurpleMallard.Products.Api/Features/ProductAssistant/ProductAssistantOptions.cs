using System;
using System.ComponentModel.DataAnnotations;

namespace PurpleMallard.Products.Api.Features.ProductAssistant;

public sealed class ProductAssistantOptions
{
    public const string SectionName = "ProductAssistant";

    [Required]
    public required AzureOpenAIOptions AzureOpenAI { get; set; }

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