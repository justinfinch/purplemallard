import { type RouteConfig, index, route } from '@react-router/dev/routes';

export default [
  // All routes are protected by the ProtectedRoute component in the root layout
  index('routes/home.tsx'),
  route('weather', 'routes/weather.tsx'),
] satisfies RouteConfig;
