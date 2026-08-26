import { apiRequest } from './apiClient';
import type { CreateLeadRequest, CreateLeadResponse } from '../types/leads';

export function createLead(request: CreateLeadRequest, signal?: AbortSignal): Promise<CreateLeadResponse> {
  return apiRequest<CreateLeadResponse>('/leads', { method: 'POST', body: request, signal });
}
