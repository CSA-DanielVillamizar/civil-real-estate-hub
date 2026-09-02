import { apiRequest } from './apiClient';
import type { LoginRequest, LoginResponse } from '../types/auth';

export function login(request: LoginRequest, signal?: AbortSignal): Promise<LoginResponse> {
  return apiRequest<LoginResponse>('/auth/login', { method: 'POST', body: request, signal });
}
