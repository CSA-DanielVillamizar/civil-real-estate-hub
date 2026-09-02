import type { ReactNode } from 'react';
import { useAuth, type AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import { LoginForm } from './LoginForm';
import { AdminNav } from './AdminNav';

interface RequireAuthProps {
  rolesPermitidos: RolUsuario[];
  children: (auth: AuthState, onUnauthorized: () => void) => ReactNode;
}

// Puerta compartida por las 3 pantallas administrativas: sin sesión, pide
// login; con sesión pero rol no autorizado (ej. AsesorComercial entrando a
// /admin/propiedades), bloquea con un mensaje en vez de dejar pasar y que
// el backend devuelva 403 en cada request — mejor UX que descubrirlo por
// llamada fallida. onUnauthorized limpia la sesión ante un 401 real del
// backend (token revocado/expirado a mitad de uso).
export function RequireAuth({ rolesPermitidos, children }: RequireAuthProps) {
  const { auth, isLoading, error, login, logout } = useAuth();

  if (!auth) {
    return <LoginForm onLogin={login} isLoading={isLoading} error={error} />;
  }

  if (!rolesPermitidos.includes(auth.rol)) {
    return (
      <div>
        <AdminNav auth={auth} onLogout={logout} />
        <div className="mx-auto max-w-lg px-6 py-16 text-center">
          <h1 className="mb-2 text-lg font-bold text-slate-900">No tienes acceso a esta sección</h1>
          <p className="text-sm text-slate-500">Tu rol ({auth.rol}) no incluye este panel.</p>
        </div>
      </div>
    );
  }

  return <>{children(auth, logout)}</>;
}
