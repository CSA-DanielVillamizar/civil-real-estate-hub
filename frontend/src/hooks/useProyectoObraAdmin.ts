import { useCallback, useEffect, useState } from 'react';
import {
  agregarEvidenciaHito,
  agregarHito,
  cambiarEstadoHito,
  cambiarEstadoProyecto,
  getProyectoObraAdmin,
} from '../services/obrasService';
import { ApiError } from '../types/api';
import type { AgregarHitoRequest, EstadoHito, EstadoProyecto, ProyectoObraDetalle } from '../types/obras';

interface UseProyectoObraAdminResult {
  proyecto: ProyectoObraDetalle | null;
  isLoading: boolean;
  error: string | null;
  busyHitoId: string | null;
  agregarHito: (request: AgregarHitoRequest) => Promise<void>;
  cambiarEstadoHito: (hitoId: string, nuevoEstado: EstadoHito) => Promise<void>;
  subirEvidencia: (hitoId: string, archivo: File) => Promise<void>;
  cambiarEstadoProyecto: (nuevoEstado: EstadoProyecto) => Promise<void>;
}

export function useProyectoObraAdmin(
  proyectoId: string,
  token: string | null,
  onUnauthorized: () => void,
): UseProyectoObraAdminResult {
  const [proyecto, setProyecto] = useState<ProyectoObraDetalle | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [busyHitoId, setBusyHitoId] = useState<string | null>(null);
  const [recargaToken, setRecargaToken] = useState(0);

  const recargar = useCallback(() => setRecargaToken((t) => t + 1), []);

  useEffect(() => {
    if (!token) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getProyectoObraAdmin(proyectoId, token, controller.signal)
      .then(setProyecto)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar el proyecto.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [proyectoId, token, recargaToken]);

  function manejarError(err: unknown, mensajePorDefecto: string) {
    if (err instanceof ApiError && err.status === 401) {
      onUnauthorized();
    } else {
      setError(err instanceof ApiError ? err.message : mensajePorDefecto);
    }
  }

  const agregarHitoFn = useCallback(
    async (request: AgregarHitoRequest) => {
      if (!token) return;
      setError(null);
      try {
        await agregarHito(proyectoId, request, token);
        recargar();
      } catch (err) {
        manejarError(err, 'No fue posible agregar el hito.');
      }
    },
    [proyectoId, token, onUnauthorized, recargar],
  );

  const cambiarEstadoHitoFn = useCallback(
    async (hitoId: string, nuevoEstado: EstadoHito) => {
      if (!token) return;
      setBusyHitoId(hitoId);
      setError(null);
      try {
        await cambiarEstadoHito(proyectoId, hitoId, nuevoEstado, token);
        recargar();
      } catch (err) {
        manejarError(err, 'No fue posible cambiar el estado del hito.');
      } finally {
        setBusyHitoId(null);
      }
    },
    [proyectoId, token, onUnauthorized, recargar],
  );

  const subirEvidenciaFn = useCallback(
    async (hitoId: string, archivo: File) => {
      if (!token) return;
      setBusyHitoId(hitoId);
      setError(null);
      try {
        await agregarEvidenciaHito(proyectoId, hitoId, archivo, token);
        recargar();
      } catch (err) {
        manejarError(err, 'No fue posible subir la evidencia.');
      } finally {
        setBusyHitoId(null);
      }
    },
    [proyectoId, token, onUnauthorized, recargar],
  );

  const cambiarEstadoProyectoFn = useCallback(
    async (nuevoEstado: EstadoProyecto) => {
      if (!token) return;
      setError(null);
      try {
        await cambiarEstadoProyecto(proyectoId, nuevoEstado, token);
        recargar();
      } catch (err) {
        manejarError(err, 'No fue posible cambiar el estado del proyecto.');
      }
    },
    [proyectoId, token, onUnauthorized, recargar],
  );

  return {
    proyecto,
    isLoading,
    error,
    busyHitoId,
    agregarHito: agregarHitoFn,
    cambiarEstadoHito: cambiarEstadoHitoFn,
    subirEvidencia: subirEvidenciaFn,
    cambiarEstadoProyecto: cambiarEstadoProyectoFn,
  };
}
