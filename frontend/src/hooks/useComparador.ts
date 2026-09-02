import { useEffect, useState } from 'react';
import { getPropertyById } from '../services/propertiesService';
import { ApiError } from '../types/api';
import type { PropertyDetailResponse } from '../types/properties';

interface UseComparadorResult {
  propiedades: PropertyDetailResponse[];
  isLoading: boolean;
  error: string | null;
}

// La ficha (GetById) trae pendiente/topografía/retiros ambientales, que la
// lista del catálogo no expone — comparar exige traer el detalle completo
// de cada propiedad seleccionada, una petición por cada una.
export function useComparador(ids: string[]): UseComparadorResult {
  const [propiedades, setPropiedades] = useState<PropertyDetailResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (ids.length === 0) {
      setPropiedades([]);
      setIsLoading(false);
      return;
    }

    const controller = new AbortController();
    setIsLoading(true);
    setError(null);

    Promise.all(ids.map((id) => getPropertyById(id, controller.signal)))
      .then(setPropiedades)
      .catch((err) => {
        if (err instanceof DOMException && err.name === 'AbortError') return;
        setError(err instanceof ApiError ? err.message : 'No fue posible cargar las propiedades a comparar.');
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ids.join(',')]);

  return { propiedades, isLoading, error };
}
