using System.Text.Json;
using FastEndpoints;
using Microsoft.Extensions.Caching.Distributed;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

// Request DTO
public sealed class GetConversationRequest
{
    public Guid ConversationId { get; set; }
}

// Response DTO  
public sealed class GetConversationResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public ConversationStatus Status { get; set; }
    public List<ConversationMessageDto> Messages { get; set; } = new();
}

public sealed class ConversationMessageDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Metadata { get; set; }
}

// FastEndpoints Endpoint
public sealed class GetConversationEndpoint : Endpoint<GetConversationRequest, GetConversationResponse>
{
    private readonly IDistributedCache _cache;

    public GetConversationEndpoint(IDistributedCache cache)
    {
        _cache = cache;
    }

    public override void Configure()
    {
        Get("/product-assistant/conversations/{ConversationId}");
        Summary(s =>
        {
            s.Summary = "Get a conversation by ID";
            s.Description = "Retrieves a conversation from Redis cache by its ID";
            s.ExampleRequest = new GetConversationRequest { ConversationId = Guid.NewGuid() };
            s.ResponseExamples[200] = new GetConversationResponse
            {
                Id = Guid.NewGuid(),
                Title = "Help me create a product description",
                CreatedAt = DateTime.UtcNow,
                UserId = "user123",
                Status = ConversationStatus.Active,
                Messages = new List<ConversationMessageDto>
                {
                    new ConversationMessageDto
                    {
                        Id = Guid.NewGuid(),
                        Content = "Hello, I need help creating a product description",
                        Role = MessageRole.User,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };
        });
    }

    public override async Task HandleAsync(GetConversationRequest req, CancellationToken ct)
    {
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
        
        // Map to response DTO
        Response = new GetConversationResponse
        {
            Id = conversation.Id,
            UserId = conversation.UserId,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            Status = conversation.Status,
            Messages = conversation.Messages.Select(m => new ConversationMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                Role = m.Role,
                CreatedAt = m.CreatedAt,
                Metadata = m.Metadata
            }).ToList()
        };
    }
}
