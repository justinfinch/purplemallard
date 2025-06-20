import React from 'react';
import { ConversationMessage, MessageRole } from '../services/product-assistant-service';

interface ChatMessageProps {
  message: ConversationMessage;
}

const ChatMessage: React.FC<ChatMessageProps> = ({ message }) => {
  const isAssistant = message.role === MessageRole.Assistant;
  const isUser = message.role === MessageRole.User;

  return (
    <div className={`flex ${isUser ? 'justify-end' : 'justify-start'} mb-6`}>
      <div className={`flex ${isUser ? 'flex-row-reverse' : 'flex-row'} max-w-4xl w-full`}>
        {/* Avatar */}
        <div className={`flex-shrink-0 ${isUser ? 'ml-3' : 'mr-3'}`}>
          <div
            className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
              isAssistant ? 'bg-orange-100 text-orange-600' : 'bg-blue-100 text-blue-600'
            }`}
          >
            {isAssistant ? 'A' : 'U'}
          </div>
        </div>

        {/* Message Content */}
        <div className={`flex-1 ${isUser ? 'text-right' : 'text-left'}`}>
          <div
            className={`inline-block max-w-full px-4 py-3 rounded-lg ${
              isAssistant
                ? 'bg-white border border-gray-200 text-gray-800'
                : 'bg-blue-600 text-white'
            }`}
          >
            <div className="whitespace-pre-wrap break-words">{message.content}</div>
          </div>
          <div className={`text-xs text-gray-500 mt-1 ${isUser ? 'text-right' : 'text-left'}`}>
            {new Date(message.createdAt).toLocaleTimeString([], {
              hour: '2-digit',
              minute: '2-digit',
            })}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatMessage;
