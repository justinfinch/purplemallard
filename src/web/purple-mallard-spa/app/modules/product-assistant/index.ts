/**
 * Main export file for product assistant module
 * This file exports all the public components, hooks, and services from the product assistant module
 */

// Product Assistant Services
export { default as productAssistantService } from './services/product-assistant-service';
export type {
  Conversation,
  ConversationMessage,
  StartConversationRequest,
  StartConversationResponse,
  GenerateCompletionRequest,
  GenerateCompletionResponse,
  ConversationStatus,
  MessageRole,
} from './services/product-assistant-service';

// Product Assistant Components
export { default as ChatInterface } from './components/ChatInterface';
export { default as NewChatPage } from './components/NewChatPage';
