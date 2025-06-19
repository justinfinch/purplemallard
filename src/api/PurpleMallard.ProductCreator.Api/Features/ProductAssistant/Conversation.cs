namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant;

public class Conversation
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public ConversationStatus Status { get; set; }
    public List<ConversationMessage> Messages { get; set; }

    public Conversation() 
    {
        Messages = new List<ConversationMessage>();
    }

    public static Conversation Create(string userId, string? title = null)
    {
        return new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? throw new ArgumentNullException(nameof(userId)),
            Title = title ?? "New Conversation",
            CreatedAt = DateTime.UtcNow,
            Status = ConversationStatus.Active,
            Messages = new List<ConversationMessage>()
        };
    }

    public void AddMessage(string content, MessageRole role, string? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty", nameof(content));

        var message = ConversationMessage.Create(Id, content, role, metadata);
        Messages.Add(message);
        LastMessageAt = DateTime.UtcNow;

        // Auto-generate title from first user message if title is default
        if (Title == "New Conversation" && role == MessageRole.User && Messages.Count == 1)
        {
            Title = content.Length > 50 ? content.Substring(0, 47) + "..." : content;
        }
    }

    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty", nameof(newTitle));
        
        Title = newTitle;
    }

    public void Archive()
    {
        Status = ConversationStatus.Archived;
    }

    public void Activate()
    {
        Status = ConversationStatus.Active;
    }
}

public class ConversationMessage
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Metadata { get; set; } // For storing additional AI model info, tokens, etc.

    public ConversationMessage() { }

    public static ConversationMessage Create(Guid conversationId, string content, MessageRole role, string? metadata = null)
    {
        return new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Content = content ?? throw new ArgumentNullException(nameof(content)),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            Metadata = metadata
        };
    }
}

public enum ConversationStatus
{
    Active,
    Archived,
    Deleted
}

public enum MessageRole
{
    User,
    Assistant,
    System
}
