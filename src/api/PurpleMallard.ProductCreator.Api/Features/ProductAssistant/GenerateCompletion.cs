using System.Text.Json;
using FastEndpoints;
using Microsoft.Extensions.Caching.Distributed;
using PurpleMallard.ProductCreator.Api.Features.ProductAssistant.Agents;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

// Request DTO
public sealed class GenerateCompletionRequest
{
    public Guid ConversationId { get; set; }
    public string Prompt { get; set; } = string.Empty;
}

// Response DTO  
public sealed class GenerateCompletionResponse
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class GenerateCompletionEndpoint : Endpoint<GenerateCompletionRequest, GenerateCompletionResponse>
{
    private readonly IDistributedCache _cache;
    private readonly IProductAssistantAgent _productAssistantAgent;

    public GenerateCompletionEndpoint(IDistributedCache cache, IProductAssistantAgent productAssistantAgent)
    {
        _cache = cache;
        _productAssistantAgent = productAssistantAgent;
    }

    public override void Configure()
    {
        Post("/product-assistant/conversations/{ConversationId}/completion");
        Summary(s =>
        {
            s.Summary = "Generate a completion for a conversation";
            s.Description = "Generates an AI completion for the given conversation using the ProductAssistantAgent and updates the conversation in cache";
            s.ExampleRequest = new GenerateCompletionRequest
            {
                ConversationId = Guid.NewGuid(),
                Prompt = "Can you help me create a product description for a wireless headphone?"
            };
            s.ResponseExamples[200] = new GenerateCompletionResponse
            {
                ConversationId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                Content = "I'd be happy to help you create a compelling product description for wireless headphones! Here's a suggested approach...",
                CreatedAt = DateTime.UtcNow
            };
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(GenerateCompletionRequest req, CancellationToken ct)
    {
        // Validate request
        if (req.ConversationId == Guid.Empty)
        {
            ThrowError("ConversationId is required");
        }

        if (string.IsNullOrWhiteSpace(req.Prompt))
        {
            ThrowError("Prompt is required");
        }

        // Get user ID from claims (assuming JWT authentication)
        var userId = User.FindFirst("UserId")?.Value ?? User.FindFirst("sub")?.Value ?? "anonymous";

        // Retrieve conversation from Redis
        var cacheKey = CacheKeys.Conversation(req.ConversationId);
        var conversationJson = await _cache.GetStringAsync(cacheKey, ct);

        if (string.IsNullOrEmpty(conversationJson))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var conversation = JsonSerializer.Deserialize<Conversation>(conversationJson);
        if (conversation == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        // Verify the conversation belongs to the current user
        if (conversation.UserId != userId)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        try
        {
            // Add the user message to the conversation
            conversation.AddMessage(req.Prompt, MessageRole.User);

            // Generate completion using the ProductAssistantAgent
            var completionContent = await _productAssistantAgent.GenerateCompletionAsync(
                conversation, 
                ct);

            // Add the assistant's response to the conversation
            conversation.AddMessage(completionContent, MessageRole.Assistant);

            // Update conversation in Redis
            var updatedConversationJson = JsonSerializer.Serialize(conversation);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                // Set expiration to 24 hours - conversations will auto-expire
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                // Extend expiration on access
                SlidingExpiration = TimeSpan.FromHours(2)
            };

            await _cache.SetStringAsync(cacheKey, updatedConversationJson, cacheOptions, ct);

            // Update user conversations list with latest message timestamp
            var userConversationsKey = CacheKeys.UserConversations(userId);
            var existingConversationsJson = await _cache.GetStringAsync(userConversationsKey, ct);

            if (!string.IsNullOrEmpty(existingConversationsJson))
            {
                var conversationSummaries = JsonSerializer.Deserialize<List<ConversationSummary>>(existingConversationsJson) ?? new List<ConversationSummary>();
                var summary = conversationSummaries.FirstOrDefault(c => c.Id == conversation.Id);
                if (summary != null)
                {
                    summary.LastMessageAt = conversation.LastMessageAt;
                    summary.Title = conversation.Title; // In case title was auto-updated
                    
                    var updatedConversationsJson = JsonSerializer.Serialize(conversationSummaries);
                    await _cache.SetStringAsync(userConversationsKey, updatedConversationsJson, cacheOptions, ct);
                }
            }

            // Get the assistant message that was just added
            var assistantMessage = conversation.Messages.Last();

            // Return response
            Response = new GenerateCompletionResponse
            {
                ConversationId = conversation.Id,
                MessageId = assistantMessage.Id,
                Content = assistantMessage.Content,
                CreatedAt = assistantMessage.CreatedAt
            };
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to inject ILogger for better logging)
            await SendResultAsync(Results.Problem(
                detail: $"Failed to generate completion: {ex.Message}",
                statusCode: 500
            ));
        }
    }
}
