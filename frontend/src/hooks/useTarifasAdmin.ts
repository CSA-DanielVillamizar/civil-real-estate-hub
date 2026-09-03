import { useCallback, useEffect, useState } from 'react';
import {
  actualizarPaqueteTarifa,
  crearPaqueteTarifa,
  despublicarPaqueteTarifa,
  getPaquetesTarifaAdmin,
  publicarPaqueteTarifa,
} from '../services/tarifasService';
import { ApiError } from '../types/api';
import type { ActualizarPaqueteTarifaRequest, CrearPaqueteTarifaRequest, PaqueteTarifa } from '../types/tarifas';

interface UseTarifasAdminResult {
  items: PaqueteTarifa[];
  isLoading: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  busyId: string | null;
  crear: (request: CrearPaqueteTarifaRequest) => Promise<boolean>;
  actualizar: (id: string, request: ActualizarPaqueteTarifaRequest) => Promise<boolean>;
  publicar: (id: string) => Promise<void>;
  despublicar: (id: string) => Promise<void>;
}

export function useTarifasAdmin(token: string | null, onUnauthorized: () => void): UseTarifasAdminResult {
  const [items, setItems] = useState<PaqueteTarifa[]>([]);
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

    getPaquetesTarifaAdmin(token, controller.signal)
      .then(setItems)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar las tarifas.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, recargaToken]);

  const crear = useCallback(
    async (request: CrearPaqueteTarifaRequest) => {
      if (!token) return false;
      setError(null);
      setFieldErrors({});

      try {
        await crearPaqueteTarifa(request, token);
        recargar();
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible crear el paquete de tarifa.');
        }
        return false;
      }
    },
    [token, onUnauthorized, recargar],
  );

  const actualizar = useCallback(
    async (id: string, request: ActualizarPaqueteTarifaRequest) => {
      if (!token) return false;
      setBusyId(id);
      setError(null);
      setFieldErrors({});

      try {
        await actualizarPaqueteTarifa(id, request, token);
        recargar();
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible actualizar el paquete de tarifa.');
        }
        return false;
      } finally {
        setBusyId(null);
      }
    },
    [token, onUnauthorized, recargar],
  );

  // Publicar/despublicar son estructuralmente idénticas — mismo helper que
  // useConfianzaAdmin.crearTransicion.
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

  const publicar = useCallback(crearTransicion(publicarPaqueteTarifa, 'No fue posible publicar el paquete de tarifa.'), [
    token,
    onUnauthorized,
    recargar,
  ]);
  const despublicar = useCallback(crearTransicion(despublicarPaqueteTarifa, 'No fue posible despublicar el paquete de tarifa.'), [
    token,
    onUnauthorized,
    recargar,
  ]);

  return { items, isLoading, error, fieldErrors, busyId, crear, actualizar, publicar, despublicar };
}
