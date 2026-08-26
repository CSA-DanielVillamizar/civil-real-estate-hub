import { useEffect, useState } from 'react';
import { getProperties } from '../services/propertiesService';
import { ApiError } from '../types/api';
import type { GetPropertiesParams, PagedPropertyResponse } from '../types/properties';

interface UsePropertiesResult {
  data: PagedPropertyResponse | null;
  isLoading: boolean;
  error: string | null;
}

export function useProperties(params: GetPropertiesParams): UsePropertiesResult {
  const [data, setData] = useState<PagedPropertyResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getProperties(params, controller.signal)
      .then(setData)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        setError(err instanceof ApiError ? err.message : 'No fue posible cargar las propiedades.');
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [JSON.stringify(params)]);

  return { data, isLoading, error };
}
