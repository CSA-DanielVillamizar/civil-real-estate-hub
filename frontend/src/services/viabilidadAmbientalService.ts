import { apiRequest, buildQueryString } from './apiClient';
import type {
  ConfirmarPagoViabilidadAmbientalResponse,
  SolicitarViabilidadAmbientalRequest,
  SolicitarViabilidadAmbientalResponse,
  SolicitudViabilidadAmbientalListItem,
} from '../types/viabilidadAmbiental';

export const ADMIN_API_KEY_HEADER = 'X-Admin-Api-Key';

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

// Ambas funciones son administrativas — requieren el mismo apiKey que
// protege los endpoints en el backend (ver AdminApiKeyEndpointFilter). Un
// 401 aquí significa que el key guardado en el navegador ya no es válido.
export function listarSolicitudesViabilidadAmbiental(
  apiKey: string,
  estado?: string,
  signal?: AbortSignal,
): Promise<SolicitudViabilidadAmbientalListItem[]> {
  const query = buildQueryString({ estado });
  return apiRequest<SolicitudViabilidadAmbientalListItem[]>(`/viabilidad-ambiental/solicitudes${query}`, {
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function confirmarPagoViabilidadAmbiental(
  id: string,
  apiKey: string,
  signal?: AbortSignal,
): Promise<ConfirmarPagoViabilidadAmbientalResponse> {
  return apiRequest<ConfirmarPagoViabilidadAmbientalResponse>(`/viabilidad-ambiental/solicitudes/${id}/confirmar-pago`, {
    method: 'POST',
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}
