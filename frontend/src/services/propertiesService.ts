import { apiRequest, authHeader, buildQueryString } from './apiClient';
import type {
  ActualizarDatosBasicosPropiedadRequest,
  ActualizarDatosBasicosPropiedadResponse,
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
// token que protege ViabilidadAmbiental (ver AdminApiKeyEndpointFilter en
// el backend, un solo mecanismo de protección en todo el sistema).
export function getPropertiesAdmin(
  token: string,
  params: GetPropertiesAdminParams = {},
  signal?: AbortSignal,
): Promise<PagedPropertyResponse> {
  const query = buildQueryString({ ...params });
  return apiRequest<PagedPropertyResponse>(`/properties/admin${query}`, {
    headers: authHeader(token),
    signal,
  });
}

export function createProperty(
  request: CrearPropiedadRequest,
  token: string,
  signal?: AbortSignal,
): Promise<CrearPropiedadResponse> {
  return apiRequest<CrearPropiedadResponse>('/properties', {
    method: 'POST',
    body: request,
    headers: authHeader(token),
    signal,
  });
}

export function agregarMultimediaAPropiedad(
  propiedadId: string,
  archivo: File,
  tipo: TipoMultimedia,
  token: string,
  signal?: AbortSignal,
): Promise<AgregarMultimediaResponse> {
  const formData = new FormData();
  formData.append('archivo', archivo);
  formData.append('tipo', tipo);

  return apiRequest<AgregarMultimediaResponse>(`/properties/${propiedadId}/multimedia`, {
    method: 'POST',
    body: formData,
    headers: authHeader(token),
    signal,
  });
}

export function publicarPropiedad(propiedadId: string, token: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/publicar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function reservarPropiedad(propiedadId: string, token: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/reservar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function marcarVendidaPropiedad(propiedadId: string, token: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/marcar-vendida`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function marcarArrendadaPropiedad(propiedadId: string, token: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/marcar-arrendada`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function retirarPropiedad(propiedadId: string, token: string, signal?: AbortSignal): Promise<PublicarPropiedadResponse> {
  return apiRequest<PublicarPropiedadResponse>(`/properties/${propiedadId}/retirar`, {
    method: 'POST',
    headers: authHeader(token),
    signal,
  });
}

export function actualizarDatosBasicosPropiedad(
  propiedadId: string,
  request: ActualizarDatosBasicosPropiedadRequest,
  token: string,
  signal?: AbortSignal,
): Promise<ActualizarDatosBasicosPropiedadResponse> {
  return apiRequest<ActualizarDatosBasicosPropiedadResponse>(`/properties/${propiedadId}`, {
    method: 'PUT',
    body: request,
    headers: authHeader(token),
    signal,
  });
}
