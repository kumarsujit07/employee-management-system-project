import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:44328/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to inject JWT authentication tokens from localStorage
api.interceptors.request.use(
  (config) => {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      try {
        const user = JSON.parse(userJson);
        if (user && user.token) {
          config.headers.Authorization = `Bearer ${user.token}`;
        }
      } catch (e) {
        console.error("Failed to parse user from localStorage", e);
      }
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor to intercept global auth failures (e.g. token expired) and redirect to login
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
