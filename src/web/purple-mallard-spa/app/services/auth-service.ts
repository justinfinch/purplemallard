/**
 * Authentication service for handling user authentication state
 * and interactions with the BFF (Backend For Frontend) authentication endpoints
 */

export interface UserClaim {
  type: string;
  value: string | number | boolean;
}

export interface User {
  name?: string;
  email?: string;
  isAuthenticated: boolean;
  claims: UserClaim[];
}

// Initial authentication state
const initialAuthState: User = {
  isAuthenticated: false,
  claims: [],
};

/**
 * Handles authentication-related API calls to the BFF endpoints
 */
export const authService = {
  /**
   * Check if the user is authenticated by calling the /bff/user endpoint
   * Returns the user if authenticated, otherwise returns null
   */
  checkAuthentication: async (): Promise<User | null> => {
    try {
      const response = await fetch('/bff/user', {
        credentials: 'include',
      });

      // Handle both 401 (Unauthorized) and 404 (Not Found) as not authenticated
      if (response.status === 401 || response.status === 404) {
        console.log(`User not authenticated. Status: ${response.status}`);
        // If 404, we need to redirect to login
        if (response.status === 404) {
          // Wait a moment before redirecting to login
          setTimeout(() => authService.login(), 100);
        }
        return null; // Not authenticated
      }

      if (!response.ok) {
        throw new Error(`Authentication check failed: ${response.statusText}`);
      }

      const claims = (await response.json()) as UserClaim[];

      // Create a user object from the claims
      const user: User = {
        isAuthenticated: true,
        claims,
      };

      // Extract common user information from claims
      for (const claim of claims) {
        if (claim.type === 'name') {
          user.name = claim.value as string;
        }
        if (claim.type === 'email') {
          user.email = claim.value as string;
        }
      }

      return user;
    } catch (error) {
      console.error('Failed to check authentication:', error);
      return null;
    }
  },

  /**
   * Redirect the user to the login page
   */
  login: async (): Promise<void> => {
    try {
      // We use window.location to perform a full redirect to the BFF login endpoint
      window.location.href = '/bff/login';
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  },

  /**
   * Logout the currently authenticated user
   */
  logout: async (): Promise<void> => {
    try {
      // Find the logout URL in the claims
      const user = await authService.checkAuthentication();
      const logoutClaim = user?.claims.find(claim => claim.type === 'bff:logout_url');

      if (logoutClaim) {
        window.location.href = logoutClaim.value as string;
      } else {
        // Fallback to the default logout URL if the claim is not found
        window.location.href = '/bff/logout';
      }
    } catch (error) {
      console.error('Logout failed:', error);
      throw error;
    }
  },
};

export default authService;
