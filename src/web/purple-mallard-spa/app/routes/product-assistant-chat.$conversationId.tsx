import { ChatInterface } from '../modules/product-assistant';

export function meta() {
  return [
    { title: 'Chat - Product Assistant' },
    { name: 'description', content: 'Chat with the Product Assistant' },
  ];
}

export default function ProductAssistantChat() {
  return <ChatInterface />;
}
