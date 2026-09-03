import { apiRequest, authHeader } from './apiClient';
import type { CrearUsuarioRequest, CrearUsuarioResponse, LoginRequest, LoginResponse, UsuarioListItem } from '../types/auth';

export function login(request: LoginRequest, signal?: AbortSignal): Promise<LoginResponse> {
  return apiRequest<LoginResponse>('/auth/login', { method: 'POST', body: request, signal });
}

// Administrativas — requieren el mismo token JWT (rol Admin) que el resto
// del panel.
export function crearUsuario(request: CrearUsuarioRequest, token: string, signal?: AbortSignal): Promise<CrearUsuarioResponse> {
  return apiRequest<CrearUsuarioResponse>('/auth/usuarios', {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function getUsuarios(token: string, signal?: AbortSignal): Promise<UsuarioListItem[]> {
  return apiRequest<UsuarioListItem[]>('/auth/usuarios', { headers: authHeader(token), signal });
}

export function cambiarActivoUsuario(
  usuarioId: string,
  activo: boolean,
  token: string,
  signal?: AbortSignal,
): Promise<{ id: string; activo: boolean }> {
  return apiRequest(`/auth/usuarios/${usuarioId}/activo`, {
    method: 'POST',
    body: { activo },
    headers: authHeader(token),
    signal,
  });
}
