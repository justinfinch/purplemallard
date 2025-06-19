using System;

namespace PurpleMallard.Products.Api.Features.ProductAssistant;

public static class CacheKeys
{
    private const string BaseKey = "product-assistant:";

    public static string Conversation(Guid conversationId)
    {
        if (conversationId == Guid.Empty)
            throw new ArgumentException("Conversation ID cannot be empty.", nameof(conversationId));
        return $"{BaseKey}conversation:{conversationId}";
    }

    public static string UserConversations(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        return $"{BaseKey}user:{userId}:conversations";
    }
}
