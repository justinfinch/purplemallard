/**
 * Main export file for auth module
 * This file exports all the public components, hooks, and services from the auth module
 */

// Auth Context
export { default as AuthContext, AuthProvider, useAuth } from './contexts/AuthContext';

// Auth Components
export { default as AuthLoading } from './components/AuthLoading';
export { default as ProtectedRoute } from './components/ProtectedRoute';
export { default as UserProfile } from './components/UserProfile';
export { default as Spinner } from './components/Spinner';

// Auth Services
export { default as authService, type User, type UserClaim } from './services/auth-service';
