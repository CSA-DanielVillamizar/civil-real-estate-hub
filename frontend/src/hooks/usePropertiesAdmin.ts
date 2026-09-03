import { useCallback, useEffect, useState } from 'react';
import {
  actualizarDatosBasicosPropiedad,
  agregarMultimediaAPropiedad,
  createProperty,
  getPropertiesAdmin,
  marcarArrendadaPropiedad,
  marcarVendidaPropiedad,
  publicarPropiedad,
  reservarPropiedad,
  retirarPropiedad,
} from '../services/propertiesService';
import { ApiError } from '../types/api';
import type { ActualizarDatosBasicosPropiedadRequest, CrearPropiedadRequest, PropertyResponse } from '../types/properties';
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
  reservar: (propiedadId: string) => Promise<void>;
  marcarVendida: (propiedadId: string) => Promise<void>;
  marcarArrendada: (propiedadId: string) => Promise<void>;
  retirar: (propiedadId: string) => Promise<void>;
  actualizarDatosBasicos: (propiedadId: string, request: ActualizarDatosBasicosPropiedadRequest) => Promise<boolean>;
}

export function usePropertiesAdmin(token: string | null, onUnauthorized: () => void): UsePropertiesAdminResult {
  const [properties, setProperties] = useState<PropertyResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [busyId, setBusyId] = useState<string | null>(null);
  const [recargaToken, setRecargaToken] = useState(0);

  const recargar = useCallback(() => setRecargaToken((t) => t + 1), []);

  useEffect(() => {
    if (!token) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getPropertiesAdmin(token, { pageSize: 100 }, controller.signal)
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
  }, [token, recargaToken]);

  const crear = useCallback(
    async (request: CrearPropiedadRequest) => {
      if (!token) return null;
      setError(null);
      setFieldErrors({});

      try {
        const result = await createProperty(request, token);
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
    [token, onUnauthorized, recargar],
  );

  const subirFoto = useCallback(
    async (propiedadId: string, archivo: File, tipo: TipoMultimedia) => {
      if (!token) return;
      setBusyId(propiedadId);
      setError(null);

      try {
        await agregarMultimediaAPropiedad(propiedadId, archivo, tipo, token);
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
    [token, onUnauthorized, recargar],
  );

  // Las 5 transiciones de estado (publicar, reservar, marcar vendida/
  // arrendada, retirar) son estructuralmente idénticas — un solo helper que
  // arma la función real de cada una, para no repetir el mismo
  // try/catch/finally 5 veces.
  function crearTransicion(accion: (propiedadId: string, token: string) => Promise<unknown>, mensajePorDefecto: string) {
    return async (propiedadId: string) => {
      if (!token) return;
      setBusyId(propiedadId);
      setError(null);

      try {
        await accion(propiedadId, token);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : mensajePorDefecto);
        }
      } finally {
        setBusyId(null);
      }
    };
  }

  const publicar = useCallback(
    crearTransicion(publicarPropiedad, 'No fue posible publicar la propiedad.'),
    [token, onUnauthorized, recargar],
  );
  const reservar = useCallback(
    crearTransicion(reservarPropiedad, 'No fue posible reservar la propiedad.'),
    [token, onUnauthorized, recargar],
  );
  const marcarVendida = useCallback(
    crearTransicion(marcarVendidaPropiedad, 'No fue posible marcar la propiedad como vendida.'),
    [token, onUnauthorized, recargar],
  );
  const marcarArrendada = useCallback(
    crearTransicion(marcarArrendadaPropiedad, 'No fue posible marcar la propiedad como arrendada.'),
    [token, onUnauthorized, recargar],
  );
  const retirar = useCallback(
    crearTransicion(retirarPropiedad, 'No fue posible retirar la propiedad.'),
    [token, onUnauthorized, recargar],
  );

  const actualizarDatosBasicos = useCallback(
    async (propiedadId: string, request: ActualizarDatosBasicosPropiedadRequest) => {
      if (!token) return false;
      setBusyId(propiedadId);
      setError(null);
      setFieldErrors({});

      try {
        await actualizarDatosBasicosPropiedad(propiedadId, request, token);
        recargar();
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible actualizar la propiedad.');
        }
        return false;
      } finally {
        setBusyId(null);
      }
    },
    [token, onUnauthorized, recargar],
  );

  return {
    properties,
    isLoading,
    error,
    fieldErrors,
    busyId,
    recargar,
    crear,
    subirFoto,
    publicar,
    reservar,
    marcarVendida,
    marcarArrendada,
    retirar,
    actualizarDatosBasicos,
  };
}
