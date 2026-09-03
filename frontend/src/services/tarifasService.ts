import { apiRequest, authHeader } from './apiClient';
import type { ActualizarPaqueteTarifaRequest, CrearPaqueteTarifaRequest, PaqueteTarifa } from '../types/tarifas';

// Público — paquetes de tarifa publicados (secciones de Consultoría/Interventoría).
export function getPaquetesTarifaPublicados(signal?: AbortSignal): Promise<PaqueteTarifa[]> {
  return apiRequest<PaqueteTarifa[]>('/paquetes-tarifa', { signal });
}

// Administrativas — mismo token JWT (rol Admin) que el resto del panel.
export function getPaquetesTarifaAdmin(token: string, signal?: AbortSignal): Promise<PaqueteTarifa[]> {
  return apiRequest<PaqueteTarifa[]>('/paquetes-tarifa/admin', { headers: authHeader(token), signal });
}

export function crearPaqueteTarifa(
  request: CrearPaqueteTarifaRequest,
  token: string,
  signal?: AbortSignal,
): Promise<PaqueteTarifa> {
  return apiRequest<PaqueteTarifa>('/paquetes-tarifa', {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function actualizarPaqueteTarifa(
  id: string,
  request: ActualizarPaqueteTarifaRequest,
  token: string,
  signal?: AbortSignal,
): Promise<PaqueteTarifa> {
  return apiRequest<PaqueteTarifa>(`/paquetes-tarifa/${id}`, {
    method: 'PUT',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function publicarPaqueteTarifa(id: string, token: string, signal?: AbortSignal): Promise<PaqueteTarifa> {
  return apiRequest<PaqueteTarifa>(`/paquetes-tarifa/${id}/publicar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function despublicarPaqueteTarifa(id: string, token: string, signal?: AbortSignal): Promise<PaqueteTarifa> {
  return apiRequest<PaqueteTarifa>(`/paquetes-tarifa/${id}/despublicar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}
