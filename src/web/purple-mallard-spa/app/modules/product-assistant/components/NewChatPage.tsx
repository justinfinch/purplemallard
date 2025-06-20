import React, { useState } from 'react';
import { useNavigate } from 'react-router';
import { productAssistantService } from '../services/product-assistant-service';
import MessageInput from './MessageInput';

const NewChatPage: React.FC = () => {
  const navigate = useNavigate();
  const [isStarting, setIsStarting] = useState(false);

  const handleStartConversation = async (initialMessage: string) => {
    try {
      setIsStarting(true);

      // Start a new conversation
      const response = await productAssistantService.startConversation({
        title:
          initialMessage.length > 50 ? initialMessage.substring(0, 47) + '...' : initialMessage,
      });

      // Navigate to the new conversation and send the initial message
      navigate(`/product-assistant/chat/${response.conversationId}`, {
        state: { initialMessage },
      });
    } catch (error) {
      console.error('Failed to start conversation:', error);
      // Could add error handling UI here
    } finally {
      setIsStarting(false);
    }
  };

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b px-4 py-3">
        <div className="max-w-4xl mx-auto flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 bg-orange-100 rounded-full flex items-center justify-center">
              <span className="text-lg font-medium text-orange-600">PA</span>
            </div>
            <h1 className="text-lg font-medium text-gray-900">Product Assistant</h1>
          </div>
          <div className="flex items-center space-x-2">
            <div className="w-2 h-2 bg-green-500 rounded-full"></div>
            <span className="text-sm text-gray-600">Ready</span>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 flex items-center justify-center px-4">
        <div className="max-w-2xl w-full text-center">
          {/* Claude AI-inspired welcome */}
          <div className="mb-8">
            <div className="w-20 h-20 bg-orange-100 rounded-full flex items-center justify-center mx-auto mb-6">
              <span className="text-3xl font-medium text-orange-600">PA</span>
            </div>
            <h1 className="text-3xl font-light text-gray-900 mb-4">How can I help you today?</h1>
            <p className="text-lg text-gray-600 mb-8">
              I'm your product assistant. I can help you create product descriptions, analyze
              features, suggest improvements, and answer questions about product development and
              marketing.
            </p>
          </div>

          {/* Example prompts */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
            <button
              onClick={() =>
                handleStartConversation(
                  'Help me create a compelling product description for a new wireless headphone'
                )
              }
              disabled={isStarting}
              className="p-4 text-left border border-gray-200 rounded-lg hover:border-gray-300 hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <div className="text-sm font-medium text-gray-900 mb-1">Product Description</div>
              <div className="text-sm text-gray-600">
                Help me create a compelling product description for a new wireless headphone
              </div>
            </button>

            <button
              onClick={() =>
                handleStartConversation(
                  'What are the key features I should highlight for a fitness tracker?'
                )
              }
              disabled={isStarting}
              className="p-4 text-left border border-gray-200 rounded-lg hover:border-gray-300 hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <div className="text-sm font-medium text-gray-900 mb-1">Feature Analysis</div>
              <div className="text-sm text-gray-600">
                What are the key features I should highlight for a fitness tracker?
              </div>
            </button>

            <button
              onClick={() =>
                handleStartConversation('How can I improve the user experience of my mobile app?')
              }
              disabled={isStarting}
              className="p-4 text-left border border-gray-200 rounded-lg hover:border-gray-300 hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <div className="text-sm font-medium text-gray-900 mb-1">UX Improvement</div>
              <div className="text-sm text-gray-600">
                How can I improve the user experience of my mobile app?
              </div>
            </button>

            <button
              onClick={() =>
                handleStartConversation('What marketing strategies work best for SaaS products?')
              }
              disabled={isStarting}
              className="p-4 text-left border border-gray-200 rounded-lg hover:border-gray-300 hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <div className="text-sm font-medium text-gray-900 mb-1">Marketing Strategy</div>
              <div className="text-sm text-gray-600">
                What marketing strategies work best for SaaS products?
              </div>
            </button>
          </div>
        </div>
      </div>

      {/* Message Input */}
      <MessageInput
        onSendMessage={handleStartConversation}
        disabled={isStarting}
        placeholder="Start a conversation with Product Assistant..."
      />

      {/* Loading overlay */}
      {isStarting && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 flex items-center space-x-3">
            <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
            <span className="text-gray-700">Starting conversation...</span>
          </div>
        </div>
      )}
    </div>
  );
};

export default NewChatPage;
