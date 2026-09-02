import { apiRequest, authHeader, buildQueryString } from './apiClient';
import type {
  ConfirmarPagoViabilidadAmbientalResponse,
  SolicitarViabilidadAmbientalRequest,
  SolicitarViabilidadAmbientalResponse,
  SolicitudViabilidadAmbientalListItem,
} from '../types/viabilidadAmbiental';

export function solicitarViabilidadAmbiental(
  request: SolicitarViabilidadAmbientalRequest,
  signal?: AbortSignal,
): Promise<SolicitarViabilidadAmbientalResponse> {
  return apiRequest<SolicitarViabilidadAmbientalResponse>('/viabilidad-ambiental/solicitudes', {
    method: 'POST',
    body: request,
    signal,
  });
}

// Ambas funciones son administrativas — requieren el mismo token JWT
// (rol Admin). Un 401 aquí significa que la sesión guardada ya no es válida.
export function listarSolicitudesViabilidadAmbiental(
  token: string,
  estado?: string,
  signal?: AbortSignal,
): Promise<SolicitudViabilidadAmbientalListItem[]> {
  const query = buildQueryString({ estado });
  return apiRequest<SolicitudViabilidadAmbientalListItem[]>(`/viabilidad-ambiental/solicitudes${query}`, {
    headers: authHeader(token),
    signal,
  });
}

export function confirmarPagoViabilidadAmbiental(
  id: string,
  token: string,
  signal?: AbortSignal,
): Promise<ConfirmarPagoViabilidadAmbientalResponse> {
  return apiRequest<ConfirmarPagoViabilidadAmbientalResponse>(`/viabilidad-ambiental/solicitudes/${id}/confirmar-pago`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}
