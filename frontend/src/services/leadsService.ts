import { apiRequest, apiRequestBlob, type BlobResponse } from './apiClient';
import type { CreateLeadRequest, CreateLeadResponse } from '../types/leads';

export function createLead(request: CreateLeadRequest, signal?: AbortSignal): Promise<CreateLeadResponse> {
  return apiRequest<CreateLeadResponse>('/leads', { method: 'POST', body: request, signal });
}

// Reutiliza el mismo CreateLeadRequest (con datosCalculoObra obligatorio en
// la práctica) — mismo contrato que usa el backend para este endpoint.
export function generarPresupuestoPdf(request: CreateLeadRequest, signal?: AbortSignal): Promise<BlobResponse> {
  return apiRequestBlob('/leads/presupuesto-pdf', { method: 'POST', body: request, signal });
}
