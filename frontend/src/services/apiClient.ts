import { ApiError, type ValidationProblemDetails } from '../types/api';

// En desarrollo, Vite hace proxy de /api hacia el backend (ver vite.config.ts).
// En producción se puede sobrescribir con VITE_API_BASE_URL.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api';

interface RequestOptions {
  method?: 'GET' | 'POST';
  body?: unknown;
  signal?: AbortSignal;
}

export async function apiRequest<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method: options.method ?? 'GET',
      headers: options.body ? { 'Content-Type': 'application/json' } : undefined,
      body: options.body ? JSON.stringify(options.body) : undefined,
      signal: options.signal,
    });
  } catch {
    throw new ApiError(0, { title: 'No se pudo conectar con el servidor. Verifica tu conexión e inténtalo de nuevo.' });
  }

  if (!response.ok) {
    let problem: ValidationProblemDetails | undefined;
    try {
      problem = (await response.json()) as ValidationProblemDetails;
    } catch {
      // Respuesta de error sin cuerpo JSON — se reporta solo el status.
    }
    throw new ApiError(response.status, problem);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}

export function buildQueryString(params: Record<string, unknown>): string {
  const query = new URLSearchParams();

  for (const [key, value] of Object.entries(params)) {
    if (value === undefined || value === null || value === '') continue;
    query.set(key, String(value));
  }

  const serialized = query.toString();
  return serialized ? `?${serialized}` : '';
}
