import React, { useState, useEffect, useRef } from 'react';
import { useNavigate, useParams, useLocation } from 'react-router';
import {
  Conversation,
  MessageRole,
  productAssistantService,
} from '../services/product-assistant-service';
import ChatMessage from './ChatMessage';
import MessageInput from './MessageInput';

const ChatInterface: React.FC = () => {
  const { conversationId } = useParams<{ conversationId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const [conversation, setConversation] = useState<Conversation | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sendingMessage, setSendingMessage] = useState(false);
  const [hasHandledInitialMessage, setHasHandledInitialMessage] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    if (conversationId) {
      loadConversation(conversationId);
    }
  }, [conversationId]);

  useEffect(() => {
    scrollToBottom();
  }, [conversation?.messages]);

  // Handle initial message from navigation state
  useEffect(() => {
    const initialMessage = location.state?.initialMessage;
    if (initialMessage && conversation && !hasHandledInitialMessage) {
      setHasHandledInitialMessage(true);
      handleSendMessage(initialMessage);
    }
  }, [conversation, location.state, hasHandledInitialMessage]);

  const loadConversation = async (id: string) => {
    try {
      setLoading(true);
      setError(null);
      const conversationData = await productAssistantService.getConversation(id);
      setConversation(conversationData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load conversation');
      console.error('Error loading conversation:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSendMessage = async (message: string) => {
    if (!conversationId || !conversation) return;

    try {
      setSendingMessage(true);
      setError(null);

      // Optimistically add user message to UI
      const userMessage = {
        id: `temp-${Date.now()}`,
        content: message,
        role: MessageRole.User,
        createdAt: new Date().toISOString(),
      };

      setConversation(prev =>
        prev
          ? {
              ...prev,
              messages: [...prev.messages, userMessage],
            }
          : null
      );

      // Send message to API
      const response = await productAssistantService.generateCompletion({
        conversationId,
        prompt: message,
      });

      // Reload conversation to get updated state
      await loadConversation(conversationId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to send message');
      console.error('Error sending message:', err);

      // Remove optimistic message on error
      setConversation(prev =>
        prev
          ? {
              ...prev,
              messages: prev.messages.filter(m => !m.id.startsWith('temp-')),
            }
          : null
      );
    } finally {
      setSendingMessage(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
          <span className="text-gray-600">Loading conversation...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center h-64 space-y-4">
        <div className="text-red-600 text-center">
          <p className="font-medium">Error loading conversation</p>
          <p className="text-sm">{error}</p>
        </div>
        <button
          onClick={() => navigate('/product-assistant/new')}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Start New Conversation
        </button>
      </div>
    );
  }

  if (!conversation) {
    return (
      <div className="flex flex-col items-center justify-center h-64 space-y-4">
        <p className="text-gray-600">Conversation not found</p>
        <button
          onClick={() => navigate('/product-assistant/new')}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Start New Conversation
        </button>
      </div>
    );
  }

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b px-4 py-3 flex items-center justify-between">
        <div className="flex items-center space-x-3">
          <button
            onClick={() => navigate('/product-assistant/new')}
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <svg
              className="w-5 h-5 text-gray-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M15 19l-7-7 7-7"
              />
            </svg>
          </button>
          <div>
            <h1 className="text-lg font-medium text-gray-900">{conversation.title}</h1>
            <p className="text-sm text-gray-500">
              {conversation.messages.length} message{conversation.messages.length !== 1 ? 's' : ''}
            </p>
          </div>
        </div>
        <div className="flex items-center space-x-2">
          <div className="w-2 h-2 bg-green-500 rounded-full"></div>
          <span className="text-sm text-gray-600">Connected</span>
        </div>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto px-4 py-6">
        <div className="max-w-4xl mx-auto">
          {conversation.messages.length === 0 ? (
            <div className="text-center py-12">
              <div className="w-16 h-16 bg-orange-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <span className="text-2xl font-medium text-orange-600">PA</span>
              </div>
              <h2 className="text-xl font-medium text-gray-900 mb-2">Product Assistant</h2>
              <p className="text-gray-600 mb-6">
                I'm here to help you with product-related questions and tasks.
              </p>
            </div>
          ) : (
            conversation.messages.map(message => <ChatMessage key={message.id} message={message} />)
          )}
          {sendingMessage && (
            <div className="flex justify-start mb-6">
              <div className="flex flex-row max-w-4xl w-full">
                <div className="flex-shrink-0 mr-3">
                  <div className="w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium bg-orange-100 text-orange-600">
                    A
                  </div>
                </div>
                <div className="flex-1">
                  <div className="inline-block px-4 py-3 rounded-lg bg-white border border-gray-200">
                    <div className="flex items-center space-x-1">
                      <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                      <div
                        className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
                        style={{ animationDelay: '0.1s' }}
                      ></div>
                      <div
                        className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
                        style={{ animationDelay: '0.2s' }}
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
        </div>
      </div>

      {/* Input */}
      <MessageInput
        onSendMessage={handleSendMessage}
        disabled={sendingMessage}
        placeholder="Ask me anything about products..."
      />
    </div>
  );
};

export default ChatInterface;
