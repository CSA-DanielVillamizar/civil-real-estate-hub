import { useCallback, useRef, useState } from 'react';
import { calculateBudget } from '../services/budgetsService';
import { ApiError } from '../types/api';
import type { DatosCalculoObra, EstimacionCosto } from '../types/common';

interface UseBudgetCalculatorResult {
  estimacion: EstimacionCosto | null;
  isCalculating: boolean;
  error: string | null;
  fieldErrors: Record<string, string[]>;
  calcular: (datos: DatosCalculoObra) => Promise<EstimacionCosto | null>;
  reset: () => void;
}

export function useBudgetCalculator(): UseBudgetCalculatorResult {
  const [estimacion, setEstimacion] = useState<EstimacionCosto | null>(null);
  const [isCalculating, setIsCalculating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string[]>>({});
  const abortRef = useRef<AbortController | null>(null);

  const calcular = useCallback(async (datos: DatosCalculoObra) => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setIsCalculating(true);
    setError(null);
    setFieldErrors({});

    try {
      const resultado = await calculateBudget(datos, controller.signal);
      setEstimacion(resultado);
      return resultado;
    } catch (err) {
      if (err instanceof DOMException && err.name === 'AbortError') return null;

      if (err instanceof ApiError) {
        setFieldErrors(err.fieldErrors());
        setError(err.message);
      } else {
        setError('No fue posible calcular la estimación. Inténtalo de nuevo.');
      }
      return null;
    } finally {
      setIsCalculating(false);
    }
  }, []);

  const reset = useCallback(() => {
    abortRef.current?.abort();
    setEstimacion(null);
    setError(null);
    setFieldErrors({});
  }, []);

  return { estimacion, isCalculating, error, fieldErrors, calcular, reset };
}
