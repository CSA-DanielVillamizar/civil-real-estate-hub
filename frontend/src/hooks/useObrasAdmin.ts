import { useCallback, useEffect, useState } from 'react';
import { crearProyectoObra, getProyectosObraAdmin } from '../services/obrasService';
import { ApiError } from '../types/api';
import type { CrearProyectoObraRequest, ProyectoObraListItem } from '../types/obras';

interface UseObrasAdminResult {
  proyectos: ProyectoObraListItem[];
  isLoading: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  crear: (request: CrearProyectoObraRequest) => Promise<CrearProyectoObraCreado | null>;
}

interface CrearProyectoObraCreado {
  id: string;
  tokenAcceso: string;
}

export function useObrasAdmin(token: string | null, onUnauthorized: () => void): UseObrasAdminResult {
  const [proyectos, setProyectos] = useState<ProyectoObraListItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [recargaToken, setRecargaToken] = useState(0);

  useEffect(() => {
    if (!token) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getProyectosObraAdmin(token, controller.signal)
      .then(setProyectos)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar los proyectos de obra.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, recargaToken]);

  const crear = useCallback(
    async (request: CrearProyectoObraRequest) => {
      if (!token) return null;
      setError(null);
      setFieldErrors({});

      try {
        const result = await crearProyectoObra(request, token);
        setRecargaToken((t) => t + 1);
        return result;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible crear el proyecto.');
        }
        return null;
      }
    },
    [token, onUnauthorized],
  );

  return { proyectos, isLoading, error, fieldErrors, crear };
}
