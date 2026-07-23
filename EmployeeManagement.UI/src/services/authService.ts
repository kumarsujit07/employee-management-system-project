import api from './api';
import { ApiResponse, User } from '../types';

export const authService = {
  async login(username: string, password: string): Promise<User> {
    const response = await api.post<ApiResponse<User>>('/auth/login', { username, password });
    const user = response.data.data;
    localStorage.setItem('user', JSON.stringify(user));
    return user;
  },

  logout(): void {
    localStorage.removeItem('user');
  },

  getCurrentUser(): User | null {
    const userJson = localStorage.getItem('user');
    if (!userJson) return null;
    try {
      return JSON.parse(userJson) as User;
    } catch {
      return null;
    }
  },

  isAuthenticated(): boolean {
    return this.getCurrentUser() !== null;
  }
};
