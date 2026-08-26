import { apiRequest, buildQueryString } from './apiClient';
import type { GetPropertiesParams, PagedPropertyResponse } from '../types/properties';

export function getProperties(params: GetPropertiesParams = {}, signal?: AbortSignal): Promise<PagedPropertyResponse> {
  const query = buildQueryString({ ...params });
  return apiRequest<PagedPropertyResponse>(`/properties${query}`, { signal });
}
