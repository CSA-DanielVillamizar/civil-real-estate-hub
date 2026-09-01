import { useCallback, useState } from 'react';

const STORAGE_KEY = 'admin-api-key';

interface UseAdminApiKeyResult {
  apiKey: string | null;
  guardar: (apiKey: string) => void;
  limpiar: () => void;
}

// Persistencia puramente local (localStorage del navegador) — el backend
// nunca ve ni valida este almacenamiento, solo el header en cada request
// (ver viabilidadAmbientalService). Si el key cambia o se revoca, limpiar()
// lo borra y la página vuelve a pedirlo.
export function useAdminApiKey(): UseAdminApiKeyResult {
  const [apiKey, setApiKey] = useState<string | null>(() => {
    try {
      return localStorage.getItem(STORAGE_KEY);
    } catch {
      return null;
    }
  });

  const guardar = useCallback((value: string) => {
    try {
      localStorage.setItem(STORAGE_KEY, value);
    } catch {
      // Almacenamiento no disponible (modo privado, etc.) — el key sigue
      // funcionando en memoria para el resto de esta sesión de la pestaña.
    }
    setApiKey(value);
  }, []);

  const limpiar = useCallback(() => {
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      // Ver nota en guardar().
    }
    setApiKey(null);
  }, []);

  return { apiKey, guardar, limpiar };
}
