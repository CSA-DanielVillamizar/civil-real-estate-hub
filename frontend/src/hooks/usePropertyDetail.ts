import { useEffect, useState } from 'react';
import { getPropertyById } from '../services/propertiesService';
import { ApiError } from '../types/api';
import type { PropertyDetailResponse } from '../types/properties';

interface UsePropertyDetailResult {
  property: PropertyDetailResponse | null;
  isLoading: boolean;
  error: string | null;
  notFound: boolean;
}

export function usePropertyDetail(id: string): UsePropertyDetailResult {
  const [property, setProperty] = useState<PropertyDetailResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);
    setNotFound(false);

    getPropertyById(id, controller.signal)
      .then(setProperty)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        if (err instanceof ApiError && err.status === 404) {
          setNotFound(true);
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar la propiedad.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, [id]);

  return { property, isLoading, error, notFound };
}
