import { apiRequest } from './apiClient';
import type { DatosCalculoObra, EstimacionCosto } from '../types/common';

export function calculateBudget(datos: DatosCalculoObra, signal?: AbortSignal): Promise<EstimacionCosto> {
  return apiRequest<EstimacionCosto>('/budgets/calculate', { method: 'POST', body: datos, signal });
}
