import { useCallback, useState } from 'react';
import { createLead } from '../services/leadsService';
import { ApiError } from '../types/api';
import type { CreateLeadRequest, CreateLeadResponse } from '../types/leads';

interface UseCreateLeadResult {
  isSubmitting: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  lead: CreateLeadResponse | null;
  enviar: (request: CreateLeadRequest) => Promise<CreateLeadResponse | null>;
  reset: () => void;
}

export function useCreateLead(): UseCreateLeadResult {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [lead, setLead] = useState<CreateLeadResponse | null>(null);

  const enviar = useCallback(async (request: CreateLeadRequest) => {
    setIsSubmitting(true);
    setError(null);
    setFieldErrors({});

    try {
      const resultado = await createLead(request);
      setLead(resultado);
      return resultado;
    } catch (err) {
      if (err instanceof ApiError) {
        setFieldErrors(err.fieldErrors());
        setError(err.message);
      } else {
        setError('No fue posible enviar tus datos. Inténtalo de nuevo.');
      }
      return null;
    } finally {
      setIsSubmitting(false);
    }
  }, []);

  const reset = useCallback(() => {
    setError(null);
    setFieldErrors({});
    setLead(null);
  }, []);

  return { isSubmitting, error, fieldErrors, lead, enviar, reset };
}
