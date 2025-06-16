import { ReactNode, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import AuthLoading from './AuthLoading';

interface ProtectedRouteProps {
  children: ReactNode;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { isAuthenticated, isLoading, login } = useAuth();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      // User is not authenticated, redirect to login
      login();
    }
  }, [isLoading, isAuthenticated, login]);

  // Show loading spinner while checking authentication
  if (isLoading) {
    return <AuthLoading />;
  }

  // If authenticated, render the children
  return isAuthenticated ? <>{children}</> : null;
}
