import { apiRequest, buildQueryString } from './apiClient';
import { ADMIN_API_KEY_HEADER } from './viabilidadAmbientalService';
import type {
  AgregarMultimediaResponse,
  CrearPropiedadRequest,
  CrearPropiedadResponse,
  GetPropertiesAdminParams,
  GetPropertiesParams,
  PagedPropertyResponse,
  PropertyDetailResponse,
  PublicarPropiedadResponse,
} from '../types/properties';
import type { TipoMultimedia } from '../types/common';

export function getProperties(params: GetPropertiesParams = {}, signal?: AbortSignal): Promise<PagedPropertyResponse> {
  const query = buildQueryString({ ...params });
  return apiRequest<PagedPropertyResponse>(`/properties${query}`, { signal });
}

export function getPropertyById(id: string, signal?: AbortSignal): Promise<PropertyDetailResponse> {
  return apiRequest<PropertyDetailResponse>(`/properties/${id}`, { signal });
}

// Todas las funciones de abajo son administrativas — requieren el mismo
// apiKey que protege ViabilidadAmbiental (ver AdminApiKeyEndpointFilter en
// el backend, un solo mecanismo de protección en todo el sistema).
export function getPropertiesAdmin(
  apiKey: string,
  params: GetPropertiesAdminParams = {},
  signal?: AbortSignal,
): Promise<PagedPropertyResponse> {
  const query = buildQueryString({ ...params });
  return apiRequest<PagedPropertyResponse>(`/properties/admin${query}`, {
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function createProperty(
  request: CrearPropiedadRequest,
  apiKey: string,
  signal?: AbortSignal,
): Promise<CrearPropiedadResponse> {
  return apiRequest<CrearPropiedadResponse>('/properties', {
    method: 'POST',
    body: request,
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function agregarMultimediaAPropiedad(
  propiedadId: string,
  archivo: File,
  tipo: TipoMultimedia,
  apiKey: string,
  signal?: AbortSignal,
): Promise<AgregarMultimediaResponse> {
  const formData = new FormData();
  formData.append('archivo', archivo);
  formData.append('tipo', tipo);

  return apiRequest<AgregarMultimediaResponse>(`/properties/${propiedadId}/multimedia`, {
    method: 'POST',
    body: formData,
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function publicarPropiedad(propiedadId: string, apiKey: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/publicar`, {
    method: 'POST',
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}
