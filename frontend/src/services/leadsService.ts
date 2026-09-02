import { apiRequest, apiRequestBlob, buildQueryString, type BlobResponse } from './apiClient';
import { ADMIN_API_KEY_HEADER } from './viabilidadAmbientalService';
import type { CreateLeadRequest, CreateLeadResponse, GetLeadsAdminParams, LeadEstadoResponse, LeadListItem } from '../types/leads';

export function createLead(request: CreateLeadRequest, signal?: AbortSignal): Promise<CreateLeadResponse> {
  return apiRequest<CreateLeadResponse>('/leads', { method: 'POST', body: request, signal });
}

// Reutiliza el mismo CreateLeadRequest (con datosCalculoObra obligatorio en
// la práctica) — mismo contrato que usa el backend para este endpoint.
export function generarPresupuestoPdf(request: CreateLeadRequest, signal?: AbortSignal): Promise<BlobResponse> {
  return apiRequestBlob('/leads/presupuesto-pdf', { method: 'POST', body: request, signal });
}

// Todas las funciones de abajo son administrativas — requieren el mismo
// apiKey que protege ViabilidadAmbiental y Properties (ver
// AdminApiKeyEndpointFilter en el backend).
export function getLeadsAdmin(
  apiKey: string,
  params: GetLeadsAdminParams = {},
  signal?: AbortSignal,
): Promise<LeadListItem[]> {
  const query = buildQueryString({ ...params });
  return apiRequest<LeadListItem[]>(`/leads/admin${query}`, {
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function marcarLeadContactado(leadId: string, apiKey: string, signal?: AbortSignal): Promise<LeadEstadoResponse> {
  return apiRequest<LeadEstadoResponse>(`/leads/${leadId}/marcar-contactado`, {
    method: 'POST',
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function calificarLead(leadId: string, apiKey: string, signal?: AbortSignal): Promise<LeadEstadoResponse> {
  return apiRequest<LeadEstadoResponse>(`/leads/${leadId}/calificar`, {
    method: 'POST',
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function convertirLead(leadId: string, apiKey: string, signal?: AbortSignal): Promise<LeadEstadoResponse> {
  return apiRequest<LeadEstadoResponse>(`/leads/${leadId}/convertir`, {
    method: 'POST',
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}

export function descartarLead(
  leadId: string,
  motivo: string,
  apiKey: string,
  signal?: AbortSignal,
): Promise<LeadEstadoResponse> {
  return apiRequest<LeadEstadoResponse>(`/leads/${leadId}/descartar`, {
    method: 'POST',
    body: { motivo },
    headers: { [ADMIN_API_KEY_HEADER]: apiKey },
    signal,
  });
}
