import { useCallback, useState } from 'react';
import { login as loginRequest } from '../services/authService';
import { ApiError } from '../types/api';
import type { RolUsuario } from '../types/auth';

const STORAGE_KEY = 'auth';

export interface AuthState {
  token: string;
  expiraEn: string;
  nombre: string;
  rol: RolUsuario;
}

interface UseAuthResult {
  auth: AuthState | null;
  isLoading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
}

// Igual que el useAdminApiKey que reemplaza: persistencia puramente local
// (localStorage del navegador), el backend nunca ve este almacenamiento.
// Si el token expiró (comparado contra expiraEn) se descarta al leerlo, sin
// esperar a que el backend lo rechace con un 401.
function leerAlmacenado(): AuthState | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;

    const parsed = JSON.parse(raw) as AuthState;
    if (new Date(parsed.expiraEn).getTime() <= Date.now()) return null;

    return parsed;
  } catch {
    return null;
  }
}

export function useAuth(): UseAuthResult {
  const [auth, setAuth] = useState<AuthState | null>(() => leerAlmacenado());
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const login = useCallback(async (email: string, password: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const result = await loginRequest({ email, password });
      const nuevo: AuthState = {
        token: result.token,
        expiraEn: result.expiraEn,
        nombre: result.nombre,
        rol: result.rol,
      };

      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(nuevo));
      } catch {
        // Almacenamiento no disponible (modo privado, etc.) — sigue
        // funcionando en memoria para el resto de esta sesión de la pestaña.
      }

      setAuth(nuevo);
      return true;
    } catch (err) {
      setError(err instanceof ApiError && err.status === 401 ? 'Email o contraseña incorrectos.' : 'No fue posible iniciar sesión.');
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      // Ver nota en login().
    }
    setAuth(null);
  }, []);

  return { auth, isLoading, error, login, logout };
}
