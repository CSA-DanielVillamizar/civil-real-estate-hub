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
  { href: '/admin/confianza', label: 'Testimonios', roles: [RolUsuario.Admin] },
  { href: '/admin/tarifas', label: 'Tarifas', roles: [RolUsuario.Admin] },
];

interface AdminNavProps {
  auth?: AuthState;
  onLogout?: () => void;
}

// Barra compartida entre las 8 pantallas administrativas — sin ella, cada
// una era una isla sin forma de llegar a las otras salvo escribiendo la URL
// a mano. Los links se filtran por rol: AsesorComercial solo ve Leads (ver
// RequireAuth, que además bloquea el acceso directo por URL).
//
// Paleta de alta densidad del "B2B Admin Console" (civil_real_estate_hub_ui/
// DESIGN.md): slate-corporate-anchor (slate-900) de fondo + engineering-cyan
// (sky-400/500) para el estado activo — reservado para el panel
// administrativo; el sitio público sigue con la barra clara.
export function AdminNav({ auth, onLogout }: AdminNavProps) {
  const path = window.location.pathname;
  const links = auth ? LINKS.filter((link) => link.roles.includes(auth.rol)) : LINKS;

  return (
    <div className="border-b border-slate-800 bg-slate-900">
      <nav className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 sm:px-6">
        <div className="flex items-center gap-1 overflow-x-auto">
          <span className="font-heading mr-2 hidden shrink-0 text-sm font-bold tracking-tight text-white sm:block">
            Plataforma <span className="text-sky-400">B2B</span>
          </span>
          {links.map((link) => {
            const activo = path === link.href || path.startsWith(`${link.href}/`);
            return (
              <a
                key={link.href}
                href={link.href}
                className={`font-heading shrink-0 whitespace-nowrap border-b-2 px-3 py-3 text-xs font-semibold uppercase tracking-wide transition ${
                  activo ? 'border-sky-400 text-white' : 'border-transparent text-slate-400 hover:text-white'
                }`}
              >
                {link.label}
              </a>
            );
          })}
        </div>

        {auth && (
          <div className="flex shrink-0 items-center gap-3 text-xs text-slate-400">
            <span className="hidden font-medium sm:inline">
              {auth.nombre} · {auth.rol}
            </span>
            <button onClick={onLogout} className="font-heading font-semibold text-slate-300 hover:text-white">
              Cerrar sesión
            </button>
          </div>
        )}
      </nav>
    </div>
  );
}
