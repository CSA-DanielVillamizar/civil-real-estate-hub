import { apiRequest, authHeader } from './apiClient';
import type {
  AgregarEvidenciaHitoResponse,
  AgregarHitoRequest,
  CrearProyectoObraRequest,
  CrearProyectoObraResponse,
  EstadoHitoResponse,
  EstadoProyectoResponse,
  HitoResponse,
  ProyectoObraDetalle,
  ProyectoObraListItem,
} from '../types/obras';

// Todas las funciones de abajo (salvo getProyectoObraPorToken) son
// administrativas — requieren el mismo token JWT (rol Admin) que el resto
// del panel.
export function crearProyectoObra(
  request: CrearProyectoObraRequest,
  token: string,
  signal?: AbortSignal,
): Promise<CrearProyectoObraResponse> {
  return apiRequest<CrearProyectoObraResponse>('/obras', {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function getProyectosObraAdmin(token: string, signal?: AbortSignal): Promise<ProyectoObraListItem[]> {
  return apiRequest<ProyectoObraListItem[]>('/obras/admin', { headers: authHeader(token), signal });
}

export function getProyectoObraAdmin(id: string, token: string, signal?: AbortSignal): Promise<ProyectoObraDetalle> {
  return apiRequest<ProyectoObraDetalle>(`/obras/admin/${id}`, { headers: authHeader(token), signal });
}

export function agregarHito(
  proyectoId: string,
  request: AgregarHitoRequest,
  token: string,
  signal?: AbortSignal,
): Promise<HitoResponse> {
  return apiRequest<HitoResponse>(`/obras/${proyectoId}/hitos`, {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function cambiarEstadoHito(
  proyectoId: string,
  hitoId: string,
  nuevoEstado: string,
  token: string,
  signal?: AbortSignal,
): Promise<EstadoHitoResponse> {
  return apiRequest<EstadoHitoResponse>(`/obras/${proyectoId}/hitos/${hitoId}/estado`, {
    method: 'POST',
    body: { nuevoEstado },
    headers: authHeader(token),
    signal,
  });
}

export function agregarEvidenciaHito(
  proyectoId: string,
  hitoId: string,
  archivo: File,
  token: string,
  signal?: AbortSignal,
): Promise<AgregarEvidenciaHitoResponse> {
  const formData = new FormData();
  formData.append('archivo', archivo);

  return apiRequest<AgregarEvidenciaHitoResponse>(`/obras/${proyectoId}/hitos/${hitoId}/evidencia`, {
    method: 'POST',
    body: formData,
    headers: authHeader(token),
    signal,
  });
}

export function cambiarEstadoProyecto(
  proyectoId: string,
  nuevoEstado: string,
  token: string,
  signal?: AbortSignal,
): Promise<EstadoProyectoResponse> {
  return apiRequest<EstadoProyectoResponse>(`/obras/${proyectoId}/estado`, {
    method: 'POST',
    body: { nuevoEstado },
    headers: authHeader(token),
    signal,
  });
}

// Público — sin headers de autenticación: el token de la URL ES la
// credencial (ver ProyectoObra.GenerarToken en el backend).
export function getProyectoObraPorToken(token: string, signal?: AbortSignal): Promise<ProyectoObraDetalle> {
  return apiRequest<ProyectoObraDetalle>(`/obras/por-token/${token}`, { signal });
}
