import { useCallback, useEffect, useState } from 'react';
import { confirmarPagoViabilidadAmbiental, listarSolicitudesViabilidadAmbiental } from '../services/viabilidadAmbientalService';
import { ApiError } from '../types/api';
import type { SolicitudViabilidadAmbientalListItem } from '../types/viabilidadAmbiental';

interface UseSolicitudesViabilidadAmbientalResult {
  solicitudes: SolicitudViabilidadAmbientalListItem[];
  isLoading: boolean;
  error: string | null;
  confirmandoId: string | null;
  recargar: () => void;
  confirmarPago: (id: string) => Promise<void>;
}

// token inválido/revocado (401 del backend) se reporta vía onUnauthorized
// en vez de guardarse como "error" normal — quien use el hook decide qué
// hacer (ViabilidadAmbientalAdminPage limpia el key guardado y vuelve a
// pedirlo), en vez de que este hook conozca de localStorage.
export function useSolicitudesViabilidadAmbiental(
  token: string | null,
  onUnauthorized: () => void,
): UseSolicitudesViabilidadAmbientalResult {
  const [solicitudes, setSolicitudes] = useState<SolicitudViabilidadAmbientalListItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [confirmandoId, setConfirmandoId] = useState<string | null>(null);
  const [recargaToken, setRecargaToken] = useState(0);

  const recargar = useCallback(() => setRecargaToken((t) => t + 1), []);

  useEffect(() => {
    if (!token) return;

    const controller = new AbortController();

    async function cargar() {
      setIsLoading(true);
      setError(null);

      try {
        const items = await listarSolicitudesViabilidadAmbiental(token!, undefined, controller.signal);
        setSolicitudes(items);
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar las solicitudes.');
        }
      } finally {
        setIsLoading(false);
      }
    }

    cargar();
    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, recargaToken]);

  const confirmarPago = useCallback(
    async (id: string) => {
      if (!token) return;

      setConfirmandoId(id);
      setError(null);

      try {
        await confirmarPagoViabilidadAmbiental(id, token);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible confirmar el pago.');
        }
      } finally {
        setConfirmandoId(null);
      }
    },
    [token, onUnauthorized, recargar],
  );

  return { solicitudes, isLoading, error, confirmandoId, recargar, confirmarPago };
}
