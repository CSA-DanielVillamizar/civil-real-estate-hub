import { apiRequest, authHeader } from './apiClient';
import type { ActualizarContenidoConfianzaRequest, ContenidoConfianza, CrearContenidoConfianzaRequest } from '../types/confianza';

// Público — testimonios y portafolio publicados (secciones del sitio).
export function getContenidoConfianzaPublicado(signal?: AbortSignal): Promise<ContenidoConfianza[]> {
  return apiRequest<ContenidoConfianza[]>('/contenido-confianza', { signal });
}

// Administrativas — mismo token JWT (rol Admin) que el resto del panel.
export function getContenidoConfianzaAdmin(token: string, signal?: AbortSignal): Promise<ContenidoConfianza[]> {
  return apiRequest<ContenidoConfianza[]>('/contenido-confianza/admin', { headers: authHeader(token), signal });
}

export function crearContenidoConfianza(
  request: CrearContenidoConfianzaRequest,
  token: string,
  signal?: AbortSignal,
): Promise<ContenidoConfianza> {
  return apiRequest<ContenidoConfianza>('/contenido-confianza', {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function actualizarContenidoConfianza(
  id: string,
  request: ActualizarContenidoConfianzaRequest,
  token: string,
  signal?: AbortSignal,
): Promise<ContenidoConfianza> {
  return apiRequest<ContenidoConfianza>(`/contenido-confianza/${id}`, {
    method: 'PUT',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function publicarContenidoConfianza(id: string, token: string, signal?: AbortSignal): Promise<ContenidoConfianza> {
  return apiRequest<ContenidoConfianza>(`/contenido-confianza/${id}/publicar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function despublicarContenidoConfianza(id: string, token: string, signal?: AbortSignal): Promise<ContenidoConfianza> {
  return apiRequest<ContenidoConfianza>(`/contenido-confianza/${id}/despublicar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}
