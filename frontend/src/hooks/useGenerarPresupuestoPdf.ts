import { useCallback, useState } from 'react';
import { descargarBlob } from '../services/apiClient';
import { generarPresupuestoPdf } from '../services/leadsService';
import { ApiError } from '../types/api';
import type { CreateLeadRequest } from '../types/leads';

interface UseGenerarPresupuestoPdfResult {
  isGenerando: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  generado: boolean;
  generar: (request: CreateLeadRequest) => Promise<boolean>;
  reset: () => void;
}

// A diferencia de useCreateLead, esta acción también registra el lead (ver
// GenerarPresupuestoPdfCommandHandler: nace Calificado) — el efecto
// secundario de "descargar el PDF en el navegador" vive aquí, no en el
// componente, para que BudgetCalculator solo orqueste UI.
export function useGenerarPresupuestoPdf(): UseGenerarPresupuestoPdfResult {
  const [isGenerando, setIsGenerando] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const [generado, setGenerado] = useState(false);

  const generar = useCallback(async (request: CreateLeadRequest) => {
    setIsGenerando(true);
    setError(null);
    setFieldErrors({});

    try {
      const { blob, fileName } = await generarPresupuestoPdf(request);
      descargarBlob(blob, fileName);
      setGenerado(true);
      return true;
    } catch (err) {
      if (err instanceof ApiError) {
        setFieldErrors(err.fieldErrors());
        setError(err.message);
      } else {
        setError('No fue posible generar el PDF. Inténtalo de nuevo.');
      }
      return false;
    } finally {
      setIsGenerando(false);
    }
  }, []);

  const reset = useCallback(() => {
    setError(null);
    setFieldErrors({});
    setGenerado(false);
  }, []);

  return { isGenerando, error, fieldErrors, generado, generar, reset };
}
