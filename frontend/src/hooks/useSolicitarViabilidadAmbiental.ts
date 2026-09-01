import { useCallback, useState } from 'react';
import { solicitarViabilidadAmbiental } from '../services/viabilidadAmbientalService';
import { ApiError } from '../types/api';
import type { SolicitarViabilidadAmbientalRequest, SolicitarViabilidadAmbientalResponse } from '../types/viabilidadAmbiental';

interface UseSolicitarViabilidadAmbientalResult {
  isSubmitting: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  resultado: SolicitarViabilidadAmbientalResponse | null;
  solicitar: (request: SolicitarViabilidadAmbientalRequest) => Promise<SolicitarViabilidadAmbientalResponse | null>;
  reset: () => void;
}

export function useSolicitarViabilidadAmbiental(): UseSolicitarViabilidadAmbientalResult {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [resultado, setResultado] = useState<SolicitarViabilidadAmbientalResponse | null>(null);

  const solicitar = useCallback(async (request: SolicitarViabilidadAmbientalRequest) => {
    setIsSubmitting(true);
    setError(null);
    setFieldErrors({});

    try {
      const respuesta = await solicitarViabilidadAmbiental(request);
      setResultado(respuesta);
      return respuesta;
    } catch (err) {
      if (err instanceof ApiError) {
        setFieldErrors(err.fieldErrors());
        setError(err.message);
      } else {
        setError('No fue posible enviar tu solicitud. Inténtalo de nuevo.');
      }
      return null;
    } finally {
      setIsSubmitting(false);
    }
  }, []);

  const reset = useCallback(() => {
    setError(null);
    setFieldErrors({});
    setResultado(null);
  }, []);

  return { isSubmitting, error, fieldErrors, resultado, solicitar, reset };
}
