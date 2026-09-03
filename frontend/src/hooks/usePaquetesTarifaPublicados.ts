import { useEffect, useState } from 'react';
import { getPaquetesTarifaPublicados } from '../services/tarifasService';
import { ApiError } from '../types/api';
import type { PaqueteTarifa } from '../types/tarifas';

interface UsePaquetesTarifaPublicadosResult {
  items: PaqueteTarifa[];
  isLoading: boolean;
  error: string | null;
}

export function usePaquetesTarifaPublicados(): UsePaquetesTarifaPublicadosResult {
  const [items, setItems] = useState<PaqueteTarifa[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getPaquetesTarifaPublicados(controller.signal)
      .then(setItems)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        setError(err instanceof ApiError ? err.message : 'No fue posible cargar las tarifas.');
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, []);

  return { items, isLoading, error };
}
