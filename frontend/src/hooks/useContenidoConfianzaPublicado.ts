import { useEffect, useState } from 'react';
import { getContenidoConfianzaPublicado } from '../services/confianzaService';
import { ApiError } from '../types/api';
import type { ContenidoConfianza } from '../types/confianza';

interface UseContenidoConfianzaPublicadoResult {
  items: ContenidoConfianza[];
  isLoading: boolean;
  error: string | null;
}

export function useContenidoConfianzaPublicado(): UseContenidoConfianzaPublicadoResult {
  const [items, setItems] = useState<ContenidoConfianza[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getContenidoConfianzaPublicado(controller.signal)
      .then(setItems)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        setError(err instanceof ApiError ? err.message : 'No fue posible cargar el contenido.');
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, []);

  return { items, isLoading, error };
}
