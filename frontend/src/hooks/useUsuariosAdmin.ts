import { useCallback, useEffect, useState } from 'react';
import { cambiarActivoUsuario, crearUsuario, getUsuarios } from '../services/authService';
import { ApiError } from '../types/api';
import type { CrearUsuarioRequest, UsuarioListItem } from '../types/auth';

interface UseUsuariosAdminResult {
  usuarios: UsuarioListItem[];
  isLoading: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  busyId: string | null;
  crear: (request: CrearUsuarioRequest) => Promise<boolean>;
  cambiarActivo: (usuarioId: string, activo: boolean) => Promise<void>;
}

export function useUsuariosAdmin(token: string | null, onUnauthorized: () => void): UseUsuariosAdminResult {
  const [usuarios, setUsuarios] = useState<UsuarioListItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [busyId, setBusyId] = useState<string | null>(null);
  const [recargaToken, setRecargaToken] = useState(0);

  useEffect(() => {
    if (!token) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getUsuarios(token, controller.signal)
      .then(setUsuarios)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar los usuarios.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, recargaToken]);

  const crear = useCallback(
    async (request: CrearUsuarioRequest) => {
      if (!token) return false;
      setError(null);
      setFieldErrors({});

      try {
        await crearUsuario(request, token);
        setRecargaToken((t) => t + 1);
        return true;
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (err instanceof ApiError) {
          setFieldErrors(err.fieldErrors());
          setError(err.message);
        } else {
          setError('No fue posible crear el usuario.');
        }
        return false;
      }
    },
    [token, onUnauthorized],
  );

  const cambiarActivo = useCallback(
    async (usuarioId: string, activo: boolean) => {
      if (!token) return;
      setBusyId(usuarioId);
      setError(null);

      try {
        await cambiarActivoUsuario(usuarioId, activo, token);
        setRecargaToken((t) => t + 1);
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible actualizar el usuario.');
        }
      } finally {
        setBusyId(null);
      }
    },
    [token, onUnauthorized],
  );

  return { usuarios, isLoading, error, fieldErrors, busyId, crear, cambiarActivo };
}
