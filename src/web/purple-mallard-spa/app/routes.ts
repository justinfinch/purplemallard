import { type RouteConfig, index, route } from '@react-router/dev/routes';

export default [
  // All routes are protected by the ProtectedRoute component in the root layout
  index('routes/home.tsx'),

  // Product Assistant routes
  route('product-assistant/new', 'routes/product-assistant-new.tsx'),
  route(
    'product-assistant/chat/:conversationId',
    'routes/product-assistant-chat.$conversationId.tsx'
  ),
] satisfies RouteConfig;
