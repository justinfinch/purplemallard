/**
 * Product Assistant service for communicating with the Products API
 */

// Base URL for API calls - adjust based on environment
// Use the same protocol, host and port that the browser is currently using
const API_BASE_URL = '/api';

/**
 * Generic fetch helper with error handling for product assistant API
 */
async function fetchProductAssistantApi<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const url = `${API_BASE_URL}/products/product-assistant${endpoint}`;

  // Determine if this is a GET request
  const method = (options.method || 'GET').toUpperCase();
  const isGetRequest = method === 'GET';

  // Build headers - exclude Content-Type for GET requests
  const headers: HeadersInit = {
    ...(options.headers as Record<string, string>),
  };

  // Only add Content-Type header for non-GET requests
  if (!isGetRequest) {
    (headers as Record<string, string>)['Content-Type'] = 'application/json';
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let errorMessage = `API request failed with status ${response.status}`;

    try {
      // Try to get error message from response body
      const errorText = await response.text();
      if (errorText) {
        errorMessage = errorText;
      }
    } catch {
      // If we can't read the response body, use the default message
    }

    throw new Error(errorMessage);
  }

  // Check if response has content before trying to parse as JSON
  const contentType = response.headers.get('content-type');
  if (!contentType || !contentType.includes('application/json')) {
    throw new Error(`Expected JSON response but received ${contentType || 'unknown content type'}`);
  }

  const responseText = await response.text();
  if (!responseText) {
    throw new Error('Response body is empty');
  }

  try {
    return JSON.parse(responseText) as T;
  } catch (error) {
    throw new Error(
      `Failed to parse JSON response: ${error instanceof Error ? error.message : 'Unknown error'}`
    );
  }
}

/**
 * Conversation status enum
 */
export enum ConversationStatus {
  Active = 0,
  Archived = 1,
  Deleted = 2,
}

/**
 * Message role enum
 */
export enum MessageRole {
  User = 0,
  Assistant = 1,
  System = 2,
}

/**
 * Conversation message data model
 */
export interface ConversationMessage {
  id: string;
  content: string;
  role: MessageRole;
  createdAt: string;
  metadata?: string;
}

/**
 * Conversation data model
 */
export interface Conversation {
  id: string;
  userId: string;
  title: string;
  createdAt: string;
  lastMessageAt?: string;
  status: ConversationStatus;
  messages: ConversationMessage[];
}

/**
 * Start conversation request model
 */
export interface StartConversationRequest {
  title?: string;
}

/**
 * Start conversation response model
 */
export interface StartConversationResponse {
  conversationId: string;
  title: string;
  createdAt: string;
  userId: string;
}

/**
 * Generate completion request model
 */
export interface GenerateCompletionRequest {
  conversationId: string;
  prompt: string;
}

/**
 * Generate completion response model
 */
export interface GenerateCompletionResponse {
  conversationId: string;
  messageId: string;
  content: string;
  createdAt: string;
}

/**
 * Product Assistant API methods
 */
export const productAssistantService = {
  /**
   * Start a new conversation with the product assistant
   */
  startConversation: (request: StartConversationRequest): Promise<StartConversationResponse> => {
    return fetchProductAssistantApi<StartConversationResponse>('/conversations', {
      method: 'POST',
      body: JSON.stringify(request),
    });
  },

  /**
   * Get a conversation by ID
   */
  getConversation: (conversationId: string): Promise<Conversation> => {
    return fetchProductAssistantApi<Conversation>(`/conversations/${conversationId}`);
  },

  /**
   * Generate a completion for a conversation
   */
  generateCompletion: (request: GenerateCompletionRequest): Promise<GenerateCompletionResponse> => {
    return fetchProductAssistantApi<GenerateCompletionResponse>(
      `/conversations/${request.conversationId}/completion`,
      {
        method: 'POST',
        body: JSON.stringify(request),
      }
    );
  },
};

export default productAssistantService;
