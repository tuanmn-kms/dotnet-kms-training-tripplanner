import api from './api';

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface LoginDto {
  usernameOrEmail: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  expiresAt: string;
}

interface RawAuthResponse {
  token?: string;
  Token?: string;
  username?: string;
  Username?: string;
  email?: string;
  Email?: string;
  expiresAt?: string;
  ExpiresAt?: string;
}

export interface User {
  id: number;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  createdAt: string;
}

const normalizeAuthResponse = (data: RawAuthResponse): AuthResponse => {
  const token = data.token ?? data.Token;
  const username = data.username ?? data.Username;
  const email = data.email ?? data.Email;
  const expiresAt = data.expiresAt ?? data.ExpiresAt;

  if (!token || !username || !email || !expiresAt) {
    throw new Error('Invalid authentication response from server.');
  }

  return {
    token,
    username,
    email,
    expiresAt,
  };
};

const authService = {
  register: async (data: RegisterDto): Promise<AuthResponse> => {
    const response = await api.post<RawAuthResponse>('https://localhost:44391/api/Auth/register', data);
    return normalizeAuthResponse(response.data);
  },

  signIn: async (data: LoginDto): Promise<AuthResponse> => {
    const response = await api.post<RawAuthResponse>('https://localhost:44391/api/Auth/login', data);
    return normalizeAuthResponse(response.data);
  },

  login: async (data: LoginDto): Promise<AuthResponse> => {
    return authService.signIn(data);
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser: (): User | null => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated: (): boolean => {
    return !!localStorage.getItem('token');
  },
};

export default authService;
