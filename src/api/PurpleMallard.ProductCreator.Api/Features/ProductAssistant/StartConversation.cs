using System.Text.Json;
using FastEndpoints;
using Microsoft.Extensions.Caching.Distributed;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

// Request DTO
public sealed class StartConversationRequest
{
    public string? Title { get; set; }
}

// Response DTO  
public sealed class StartConversationResponse
{
    public Guid ConversationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
}

// Supporting DTO for conversation summaries
public sealed class ConversationSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
}

// FastEndpoints Endpoint
public sealed class StartConversationEndpoint : Endpoint<StartConversationRequest, StartConversationResponse>
{
    private readonly IDistributedCache _cache;

    public StartConversationEndpoint(IDistributedCache cache)
    {
        _cache = cache;
    }

    public override void Configure()
    {
        Post("/product-assistant/conversations");
        Summary(s =>
        {
            s.Summary = "Start a new conversation with the product assistant";
            s.Description = "Creates a new conversation thread and stores it in Redis cache";
            s.ExampleRequest = new StartConversationRequest { Title = "Help me create a product description" };
            s.ResponseExamples[200] = new StartConversationResponse
            {
                ConversationId = Guid.NewGuid(),
                Title = "Help me create a product description",
                CreatedAt = DateTime.UtcNow,
                UserId = "user123"
            };
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(StartConversationRequest req, CancellationToken ct)
    {
        // Get user ID from claims (assuming JWT authentication)
        var userId = User.FindFirst("UserId")?.Value ?? User.FindFirst("sub")?.Value ?? "anonymous";

        // Create new conversation
        var conversation = Conversation.Create(userId, req.Title);

        // Store conversation in Redis with a key that includes the conversation ID
        var cacheKey = CacheKeys.Conversation(conversation.Id);
        var conversationJson = JsonSerializer.Serialize(conversation);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            // Set expiration to 24 hours - conversations will auto-expire
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
            // Extend expiration on access
            SlidingExpiration = TimeSpan.FromHours(2)
        };

        await _cache.SetStringAsync(cacheKey, conversationJson, cacheOptions, ct);

        // Also store a user-conversations list for easy retrieval
        var userConversationsKey = CacheKeys.UserConversations(userId);
        var existingConversationsJson = await _cache.GetStringAsync(userConversationsKey, ct);

        var conversationSummaries = new List<ConversationSummary>();
        if (!string.IsNullOrEmpty(existingConversationsJson))
        {
            conversationSummaries = JsonSerializer.Deserialize<List<ConversationSummary>>(existingConversationsJson) ?? new List<ConversationSummary>();
        }

        conversationSummaries.Add(new ConversationSummary
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt
        });

        var updatedConversationsJson = JsonSerializer.Serialize(conversationSummaries);
        await _cache.SetStringAsync(userConversationsKey, updatedConversationsJson, cacheOptions, ct);

        // Return response
        Response = new StartConversationResponse
        {
            ConversationId = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UserId = conversation.UserId
        };
    }
}
