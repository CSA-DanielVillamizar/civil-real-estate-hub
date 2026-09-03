import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';

interface Link {
  href: string;
  label: string;
  roles: RolUsuario[];
}

const LINKS: Link[] = [
  { href: '/admin/leads', label: 'Leads', roles: [RolUsuario.Admin, RolUsuario.AsesorComercial] },
  { href: '/admin/propiedades', label: 'Propiedades', roles: [RolUsuario.Admin] },
  { href: '/admin/viabilidad-ambiental', label: 'Viabilidad ambiental', roles: [RolUsuario.Admin] },
  { href: '/admin/obras', label: 'Avance de obra', roles: [RolUsuario.Admin] },
  { href: '/admin/usuarios', label: 'Usuarios', roles: [RolUsuario.Admin] },
];

interface AdminNavProps {
  auth?: AuthState;
  onLogout?: () => void;
}

// Barra compartida entre las 3 pantallas administrativas — sin ella, cada
// una era una isla sin forma de llegar a las otras salvo escribiendo la URL
// a mano. Los links se filtran por rol: AsesorComercial solo ve Leads (ver
// RequireAuth, que además bloquea el acceso directo por URL).
export function AdminNav({ auth, onLogout }: AdminNavProps) {
  const path = window.location.pathname;
  const links = auth ? LINKS.filter((link) => link.roles.includes(auth.rol)) : LINKS;

  return (
    <div className="border-b border-slate-200 bg-white">
      <nav className="mx-auto flex max-w-6xl items-center justify-between px-6">
        <div className="flex gap-1">
          {links.map((link) => (
            <a
              key={link.href}
              href={link.href}
              className={`border-b-2 px-3 py-3 text-sm font-medium transition ${
                path === link.href || path.startsWith(`${link.href}/`)
                  ? 'border-emerald-600 text-emerald-700'
                  : 'border-transparent text-slate-500 hover:text-slate-900'
              }`}
            >
              {link.label}
            </a>
          ))}
        </div>

        {auth && (
          <div className="flex items-center gap-3 text-sm text-slate-500">
            <span>
              {auth.nombre} · {auth.rol}
            </span>
            <button onClick={onLogout} className="font-medium text-slate-700 hover:text-slate-900">
              Cerrar sesión
            </button>
          </div>
        )}
      </nav>
    </div>
  );
}
