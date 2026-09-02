import { useCallback, useEffect, useState } from 'react';
import { calificarLead, convertirLead, descartarLead, getLeadsAdmin, marcarLeadContactado } from '../services/leadsService';
import { ApiError } from '../types/api';
import type { EstadoLead } from '../types/common';
import type { LeadListItem } from '../types/leads';

interface UseLeadsAdminResult {
  leads: LeadListItem[];
  isLoading: boolean;
  error: string | null;
  busyId: string | null;
  filtro: EstadoLead | '';
  setFiltro: (estado: EstadoLead | '') => void;
  recargar: () => void;
  marcarContactado: (leadId: string) => Promise<void>;
  calificar: (leadId: string) => Promise<void>;
  convertir: (leadId: string) => Promise<void>;
  descartar: (leadId: string, motivo: string) => Promise<void>;
}

export function useLeadsAdmin(token: string | null, onUnauthorized: () => void): UseLeadsAdminResult {
  const [leads, setLeads] = useState<LeadListItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [busyId, setBusyId] = useState<string | null>(null);
  const [filtro, setFiltro] = useState<EstadoLead | ''>('');
  const [recargaToken, setRecargaToken] = useState(0);

  const recargar = useCallback(() => setRecargaToken((t) => t + 1), []);

  useEffect(() => {
    if (!token) return;
    const controller = new AbortController();

    setIsLoading(true);
    setError(null);

    getLeadsAdmin(token, { estado: filtro || undefined }, controller.signal)
      .then(setLeads)
      .catch((err) => {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setError(err instanceof ApiError ? err.message : 'No fue posible cargar los leads.');
        }
      })
      .finally(() => setIsLoading(false));

    return () => controller.abort();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, filtro, recargaToken]);

  function ejecutarAccion(accion: (leadId: string, tokenValue: string) => Promise<unknown>) {
    return async (leadId: string) => {
      if (!token) return;
      setBusyId(leadId);
      setError(null);

      try {
        await accion(leadId, token);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible completar la acción.');
        }
      } finally {
        setBusyId(null);
      }
    };
  }

  const marcarContactado = useCallback(ejecutarAccion((id, key) => marcarLeadContactado(id, key)), [token, onUnauthorized]);
  const calificar = useCallback(ejecutarAccion((id, key) => calificarLead(id, key)), [token, onUnauthorized]);
  const convertir = useCallback(ejecutarAccion((id, key) => convertirLead(id, key)), [token, onUnauthorized]);

  const descartar = useCallback(
    async (leadId: string, motivo: string) => {
      if (!token) return;
      setBusyId(leadId);
      setError(null);

      try {
        await descartarLead(leadId, motivo, token);
        recargar();
      } catch (err) {
        if (err instanceof ApiError && err.status === 401) {
          onUnauthorized();
        } else {
          setError(err instanceof ApiError ? err.message : 'No fue posible descartar el lead.');
        }
      } finally {
        setBusyId(null);
      }
    },
    [token, onUnauthorized, recargar],
  );

  return { leads, isLoading, error, busyId, filtro, setFiltro, recargar, marcarContactado, calificar, convertir, descartar };
}
