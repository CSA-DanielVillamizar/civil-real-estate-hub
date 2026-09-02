import { ApiError, type ValidationProblemDetails } from '../types/api';

// En desarrollo, Vite hace proxy de /api hacia el backend (ver vite.config.ts).
// En producción se puede sobrescribir con VITE_API_BASE_URL.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api';

interface RequestOptions {
  method?: 'GET' | 'POST';
  body?: unknown;
  signal?: AbortSignal;
  headers?: Record<string, string>;
}

// Primitiva compartida: hace el fetch, traduce errores de red/HTTP a ApiError
// y devuelve el Response ya validado (2xx) — apiRequest lo lee como JSON,
// apiRequestBlob (descargas de archivos) lo lee como blob.
async function fetchOk(path: string, options: RequestOptions): Promise<Response> {
  let response: Response;

  try {
    // FormData (subida de archivos) viaja tal cual — el navegador arma el
    // header Content-Type con el boundary correcto; fijarlo a mano rompería
    // el multipart/form-data.
    const isFormData = options.body instanceof FormData;

    response = await fetch(`${API_BASE_URL}${path}`, {
      method: options.method ?? 'GET',
      headers: {
        ...(options.body && !isFormData ? { 'Content-Type': 'application/json' } : undefined),
        ...options.headers,
      },
      body: isFormData ? (options.body as FormData) : options.body ? JSON.stringify(options.body) : undefined,
      signal: options.signal,
    });
  } catch (err) {
    // Un abort intencional (ej. limpieza de useEffect en StrictMode, o el
    // usuario navega antes de que responda) no es un error de red real — se
    // deja propagar tal cual para que el llamador lo distinga (ver
    // useSolicitudesViabilidadAmbiental) en vez de mostrarlo como falla.
    if (err instanceof DOMException && err.name === 'AbortError') throw err;
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

  return response;
}

export async function apiRequest<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
  const response = await fetchOk(path, options);

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}

export interface BlobResponse {
  blob: Blob;
  fileName: string;
}

const DEFAULT_FILE_NAME = 'archivo';

export async function apiRequestBlob(path: string, options: RequestOptions = {}): Promise<BlobResponse> {
  const response = await fetchOk(path, options);
  const blob = await response.blob();
  const fileName = fileNameFromContentDisposition(response.headers.get('Content-Disposition')) ?? DEFAULT_FILE_NAME;

  return { blob, fileName };
}

function fileNameFromContentDisposition(header: string | null): string | null {
  if (!header) return null;

  const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(header);
  return match ? decodeURIComponent(match[1]) : null;
}

// Dispara la descarga del blob en el navegador — crea un <a> temporal, nunca
// visible en el DOM final.
export function descargarBlob(blob: Blob, fileName: string): void {
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
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
