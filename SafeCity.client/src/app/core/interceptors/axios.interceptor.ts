import axios from 'axios';
import { getToken, clearTokens } from '../../shared/auth-utils';

// Runs once at app startup (called in main.ts).
// Attaches Bearer token to every axios request automatically.
export function setupAxiosInterceptors(): void {
  axios.interceptors.request.use((config) => {
    const token = getToken();
    if (token && config.headers) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  });

  axios.interceptors.response.use(
    (response) => response,
    (error) => {
      // If token expired, clear everything and let guard redirect to login
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        clearTokens();
      }
      return Promise.reject(error);
    }
  );
}
