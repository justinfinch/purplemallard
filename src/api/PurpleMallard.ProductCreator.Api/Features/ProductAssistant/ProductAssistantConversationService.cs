using System;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace PurpleMallard.ProductCreator.Api.Features.ProductAssistant.Agents;

/// <summary>
/// Handles product-related conversations and coordinates with the other product assistant agents.
/// This agent is responsible for managing the conversation flow, understanding user queries,
/// and delegating tasks to specialized agents as needed.
/// It serves as the main entry point for product-related interactions, ensuring that user requests
/// are addressed efficiently and effectively.
/// It may also handle context management, keeping track of the conversation history and user preferences.
/// This agent is designed to work in conjunction with other agents, such as the ProductCreationAgent,
/// ProductRecommendationAgent, and ProductFeedbackAgent, to provide a comprehensive product assistant experience.
/// </summary>
public interface IProductAssistantConversationService
{

    Task<string> GenerateCompletionAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default);
}

public class ProductAssistantConversationService(
    [FromKeyedServices(Constants.ProductAssistantServiceKeys.ProductAssistantKernel)] Kernel kernel) : IProductAssistantConversationService
{
    public async Task<string> GenerateCompletionAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default)
    {
        IChatCompletionService chatCompletionService = kernel.Services
            .GetRequiredKeyedService<IChatCompletionService>(Constants.ProductAssistantServiceKeys.ProductAssistantLlm);

        // Build a chat history from the conversation. Only use the last 10 messages to avoid context overflow.
        // If the conversation has more than 10 messages, take the last 10
        ChatHistory chatHistory = new();
        foreach (var m in conversation.Messages.OrderByDescending(m => m.CreatedAt).Take(10))
        {
            ChatMessageContent chatMessage = m.Role switch
            {
                MessageRole.User => new ChatMessageContent(AuthorRole.User, m.Content),
                MessageRole.Assistant => new ChatMessageContent(AuthorRole.Assistant, m.Content),
                _ => new ChatMessageContent(AuthorRole.System, m.Content)
            };
            chatHistory.Add(chatMessage);
        }

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        // Get the chat message content
        var results = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel
        );
        
        // Return the generated response if available
        return results.Content ?? string.Empty;
    }
}
