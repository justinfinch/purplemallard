import { NewChatPage } from '../modules/product-assistant';

export function meta() {
  return [
    { title: 'New Chat - Product Assistant' },
    { name: 'description', content: 'Start a new conversation with the Product Assistant' },
  ];
}

export default function ProductAssistantNew() {
  return <NewChatPage />;
}
