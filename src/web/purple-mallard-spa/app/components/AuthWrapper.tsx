import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import AuthLoading from './AuthLoading';

interface AuthWrapperProps {
  children: React.ReactNode;
}

/**
 * Component that wraps content and only displays it when authentication status is settled
 */
export default function AuthWrapper({ children }: AuthWrapperProps) {
  const { isLoading } = useAuth();

  if (isLoading) {
    return <AuthLoading />;
  }

  return <>{children}</>;
}
