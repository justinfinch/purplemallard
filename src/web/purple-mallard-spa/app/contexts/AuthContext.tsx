import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import authService, { User } from '../services/auth-service';

interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
}

// Create the Auth Context with a default value
const AuthContext = createContext<AuthContextType>({
  user: null,
  isLoading: true,
  isAuthenticated: false,
  login: async () => {},
  logout: async () => {},
  refreshUser: async () => {},
});

// Provider component to wrap the application and provide auth state
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Function to refresh the user data
  const refreshUser = async () => {
    setIsLoading(true);
    try {
      const userData = await authService.checkAuthentication();
      setUser(userData);
    } catch (error) {
      console.error('Failed to refresh user data:', error);
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  };

  // Check authentication status on component mount
  useEffect(() => {
    refreshUser();
  }, []);

  // Login function that will redirect the user to the login page
  const login = async () => {
    await authService.login();
  };

  // Logout function that will redirect the user to the logout page
  const logout = async () => {
    await authService.logout();
    setUser(null);
  };

  // Create the context value to be provided to consumers
  const contextValue: AuthContextType = {
    user,
    isLoading,
    isAuthenticated: !!user?.isAuthenticated,
    login,
    logout,
    refreshUser,
  };

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
}

// Custom hook to use the auth context
export function useAuth() {
  return useContext(AuthContext);
}

export default AuthContext;
