import { useEffect, useState } from 'react';
import { getProyectoObraPorToken } from '../services/obrasService';
import { ApiError } from '../types/api';
import type { ProyectoObraDetalle } from '../types/obras';

interface UseMiObraResult {
  proyecto: ProyectoObraDetalle | null;
  isLoading: boolean;
  notFound: boolean;
  error: string | null;
}

export function useMiObra(token: string): UseMiObraResult {
  const [proyecto, setProyecto] = useState<ProyectoObraDetalle | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);
    setNotFound(false);

    getProyectoObraPorToken(token, controller.signal)
      .then(setProyecto)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        if (err instanceof ApiError && err.status === 404) {
          setNotFound(true);
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar el proyecto.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, [token]);

  return { proyecto, isLoading, notFound, error };
}
