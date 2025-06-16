import { useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';

/**
 * Hook to check authentication status and perform appropriate actions
 */
export default function useAuthCheck() {
  const { isLoading, isAuthenticated, login, refreshUser } = useAuth();

  // Check authentication on mount and periodically refresh
  useEffect(() => {
    // Initial authentication check is handled by the AuthContext

    // Set up periodic refresh to keep the session alive and update user data
    const refreshInterval = setInterval(() => {
      refreshUser();
    }, 5 * 60 * 1000); // Check every 5 minutes

    return () => {
      clearInterval(refreshInterval);
    };
  }, [refreshUser]);

  // Handle authentication redirects
  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      // If not authenticated and not currently checking, redirect to login
      login();
    }
  }, [isLoading, isAuthenticated, login]);

  return { isLoading, isAuthenticated };
}
