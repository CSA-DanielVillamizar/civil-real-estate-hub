import { useCallback, useEffect, useState } from 'react';
import { agregarMultimediaAPropiedad, createProperty, getPropertiesAdmin, publicarPropiedad } from '../services/propertiesService';
import { ApiError } from '../types/api';
import type { CrearPropiedadRequest, PropertyResponse } from '../types/properties';
import type { TipoMultimedia } from '../types/common';

interface UsePropertiesAdminResult {
  properties: PropertyResponse[];
  isLoading: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  busyId: string | null;
  recargar: () => void;
  crear: (request: CrearPropiedadRequest) => Promise<string | null>;
  subirFoto: (propiedadId: string, archivo: File, tipo: TipoMultimedia) => Promise<void>;
  publicar: (propiedadId: string) => Promise<void>;
}

export function usePropertiesAdmin(apiKey: string | null, onUnauthorized: () => void): UsePropertiesAdminResult {
  const [properties, setProperties] = useState<PropertyResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [busyId, setBusyId] = useState<string | null>(null);
  const [recargaToken, setRecargaToken] = useState(0);

  const recargar = useCallback(() => setRecargaToken((t) => t + 1), []);

  useEffect(() => {
    if (!apiKey) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getPropertiesAdmin(apiKey, { pageSize: 100 }, controller.signal)
      .then((data) => setProperties(data.items))
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar las propiedades.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [apiKey, recargaToken]);

  const crear = useCallback(
    async (request: CrearPropiedadRequest) => {
      if (!apiKey) return null;
      setError(null);
      setFieldErrors({});

      try {
        const result = await createProperty(request, apiKey);
        recargar();
        return result.id;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible crear la propiedad.');
        }
        return null;
      }
    },
    [apiKey, onUnauthorized, recargar],
  );

  const subirFoto = useCallback(
    async (propiedadId: string, archivo: File, tipo: TipoMultimedia) => {
      if (!apiKey) return;
      setBusyId(propiedadId);
      setError(null);

      try {
        await agregarMultimediaAPropiedad(propiedadId, archivo, tipo, apiKey);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible subir la foto.');
        }
      } finally {
        setBusyId(null);
      }
    },
    [apiKey, onUnauthorized, recargar],
  );

  const publicar = useCallback(
    async (propiedadId: string) => {
      if (!apiKey) return;
      setBusyId(propiedadId);
      setError(null);

      try {
        await publicarPropiedad(propiedadId, apiKey);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible publicar la propiedad.');
        }
      } finally {
        setBusyId(null);
      }
    },
    [apiKey, onUnauthorized, recargar],
  );

  return { properties, isLoading, error, fieldErrors, busyId, recargar, crear, subirFoto, publicar };
}
