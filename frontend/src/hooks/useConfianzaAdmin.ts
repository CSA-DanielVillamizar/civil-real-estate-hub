import { useCallback, useEffect, useState } from 'react';
import {
  actualizarContenidoConfianza,
  crearContenidoConfianza,
  despublicarContenidoConfianza,
  getContenidoConfianzaAdmin,
  publicarContenidoConfianza,
} from '../services/confianzaService';
import { ApiError } from '../types/api';
import type { ActualizarContenidoConfianzaRequest, ContenidoConfianza, CrearContenidoConfianzaRequest } from '../types/confianza';

interface UseConfianzaAdminResult {
  items: ContenidoConfianza[];
  isLoading: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  busyId: string | null;
  crear: (request: CrearContenidoConfianzaRequest) => Promise<boolean>;
  actualizar: (id: string, request: ActualizarContenidoConfianzaRequest) => Promise<boolean>;
  publicar: (id: string) => Promise<void>;
  despublicar: (id: string) => Promise<void>;
}

export function useConfianzaAdmin(token: string | null, onUnauthorized: () => void): UseConfianzaAdminResult {
  const [items, setItems] = useState<ContenidoConfianza[]>([]);
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

    getContenidoConfianzaAdmin(token, controller.signal)
      .then(setItems)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar el contenido.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, recargaToken]);

  const crear = useCallback(
    async (request: CrearContenidoConfianzaRequest) => {
      if (!token) return false;
      setError(null);
      setFieldErrors({});

      try {
        await crearContenidoConfianza(request, token);
        recargar();
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible crear el contenido.');
        }
        return false;
      }
    },
    [token, onUnauthorized, recargar],
  );

  const actualizar = useCallback(
    async (id: string, request: ActualizarContenidoConfianzaRequest) => {
      if (!token) return false;
      setBusyId(id);
      setError(null);
      setFieldErrors({});

      try {
        await actualizarContenidoConfianza(id, request, token);
        recargar();
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible actualizar el contenido.');
        }
        return false;
      } finally {
        setBusyId(null);
      }
    },
    [token, onUnauthorized, recargar],
  );

  // Publicar/despublicar son estructuralmente idénticas — un solo helper
  // que arma la función real de cada una (mismo patrón que
  // usePropertiesAdmin.crearTransicion).
  function crearTransicion(accion: (id: string, token: string) => Promise<unknown>, mensajePorDefecto: string) {
    return async (id: string) => {
      if (!token) return;
      setBusyId(id);
      setError(null);

      try {
        await accion(id, token);
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

  const publicar = useCallback(crearTransicion(publicarContenidoConfianza, 'No fue posible publicar el contenido.'), [
    token,
    onUnauthorized,
    recargar,
  ]);
  const despublicar = useCallback(crearTransicion(despublicarContenidoConfianza, 'No fue posible despublicar el contenido.'), [
    token,
    onUnauthorized,
    recargar,
  ]);

  return { items, isLoading, error, fieldErrors, busyId, crear, actualizar, publicar, despublicar };
}
