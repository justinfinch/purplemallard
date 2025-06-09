/**
 * API service for communicating with the ASP.NET Core backend
 */

// Base URL for API calls - adjust based on environment
// Use the same protocol, host and port that the browser is currently using
const API_BASE_URL = import.meta.env.DEV ? `http://localhost:5023/api` : '/api';

/**
 * Generic fetch helper with error handling
 */
async function fetchApi<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || `API request failed with status ${response.status}`);
  }

  return (await response.json()) as T;
}

/**
 * Weather forecast data model (matching ASP.NET Core API model)
 */
export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

/**
 * API methods for interacting with the backend
 */
export const apiService = {
  /**
   * Get weather forecast data from the API
   */
  getWeatherForecast: () => {
    return fetchApi<WeatherForecast[]>('/weatherforecast');
  },

  // Add more API methods as needed
};

export default apiService;
