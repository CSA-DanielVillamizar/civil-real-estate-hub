import { useState, type FormEvent } from 'react';
import { useUsuariosAdmin } from '../../hooks/useUsuariosAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import type { UsuarioListItem } from '../../types/auth';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

export function UsuariosAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <Panel auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const { usuarios, isLoading, error, fieldErrors, busyId, crear, cambiarActivo } = useUsuariosAdmin(auth.token, onUnauthorized);

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-3xl px-6 py-10">
        <h1 className="font-heading mb-1 text-2xl font-bold tracking-tight text-slate-900">Usuarios del equipo</h1>
        <p className="mb-6 text-sm text-slate-500">
          Crea cuentas para tus asesores comerciales o para otros administradores. Los Asesores Comerciales solo ven
          el panel de Leads.
        </p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        <div className="mb-8">
          <CrearUsuarioForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {!isLoading && usuarios.length > 0 && <ResumenUsuarios usuarios={usuarios} />}

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : usuarios.length === 0 ? (
          <p className="text-sm text-slate-500">Aún no hay usuarios (raro — al menos tu cuenta debería aparecer aquí).</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
            <table className="w-full min-w-[600px] table-fixed border-collapse text-left text-sm">
              <thead className="bg-slate-50 text-left text-[11px] font-bold uppercase tracking-wide text-slate-500">
                <tr>
                  <th className="w-64 px-3 py-2">Usuario</th>
                  <th className="w-40 px-3 py-2">Rol</th>
                  <th className="w-28 px-3 py-2">Estado</th>
                  <th className="px-3 py-2 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {usuarios.map((u) => (
                  <FilaUsuario key={u.id} usuario={u} busy={busyId === u.id} onCambiarActivo={cambiarActivo} />
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

// Tarjetas de resumen — agregados reales sobre los usuarios ya cargados,
// para ver de un vistazo el estado y la composición del equipo.
function ResumenUsuarios({ usuarios }: { usuarios: UsuarioListItem[] }) {
  const activos = usuarios.filter((u) => u.activo).length;
  const inactivos = usuarios.length - activos;
  const admins = usuarios.filter((u) => u.rol === RolUsuario.Admin).length;

  const tarjetas = [
    { etiqueta: 'Total usuarios', valor: usuarios.length },
    { etiqueta: 'Cuentas activas', valor: activos },
    { etiqueta: 'Cuentas inactivas', valor: inactivos },
    { etiqueta: 'Admins', valor: admins },
  ];

  return (
    <div className="mb-6 grid grid-cols-2 gap-3 sm:grid-cols-4">
      {tarjetas.map((t) => (
        <div key={t.etiqueta} className="rounded-xl border border-slate-200 bg-white p-4">
          <p className="font-heading text-[11px] font-bold uppercase tracking-wide text-slate-500">{t.etiqueta}</p>
          <p className="font-heading mt-1 text-2xl font-bold tracking-tight text-slate-900">{t.valor}</p>
        </div>
      ))}
    </div>
  );
}

function FilaUsuario({
  usuario,
  busy,
  onCambiarActivo,
}: {
  usuario: UsuarioListItem;
  busy: boolean;
  onCambiarActivo: (id: string, activo: boolean) => Promise<void>;
}) {
  return (
    <tr className="align-top hover:bg-slate-50">
      <td className="px-3 py-2">
        <p className="font-heading truncate font-semibold text-slate-900">{usuario.nombre}</p>
        <p className="truncate text-xs text-slate-500">{usuario.email}</p>
      </td>
      <td className="px-3 py-2 text-slate-600">{usuario.rol}</td>
      <td className="px-3 py-2">
        <span
          className={`font-heading rounded-full px-2 py-1 text-[11px] font-bold uppercase tracking-wide ${
            usuario.activo ? 'bg-emerald-100 text-emerald-800' : 'bg-slate-100 text-slate-500'
          }`}
        >
          {usuario.activo ? 'Activo' : 'Inactivo'}
        </span>
      </td>
      <td className="px-3 py-2 text-right">
        <button
          type="button"
          onClick={() => onCambiarActivo(usuario.id, !usuario.activo)}
          disabled={busy}
          className="rounded-md border border-slate-300 px-2 py-1 text-[11px] font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          {busy ? '…' : usuario.activo ? 'Desactivar' : 'Activar'}
        </button>
      </td>
    </tr>
  );
}

function CrearUsuarioForm({
  fieldErrors,
  onCrear,
}: {
  fieldErrors: Record<string, string[]>;
  onCrear: (request: { nombre: string; email: string; password: string; rol: RolUsuario }) => Promise<boolean>;
}) {
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rol, setRol] = useState<RolUsuario>(RolUsuario.AsesorComercial);
  const [creando, setCreando] = useState(false);
  const [creado, setCreado] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setCreando(true);
    setCreado(false);

    const ok = await onCrear({ nombre, email, password, rol });

    setCreando(false);
    if (ok) {
      setNombre('');
      setEmail('');
      setPassword('');
      setRol(RolUsuario.AsesorComercial);
      setCreado(true);
    }
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5">
      <h3 className="font-heading mb-3 font-semibold text-slate-900">Nuevo usuario</h3>

      {creado && (
        <div className="mb-4 rounded-md border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-900">
          Usuario creado. Comparte el email y la contraseña con esa persona por un canal seguro (no por este panel).
        </div>
      )}

      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <div>
          <input
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            placeholder="Nombre"
            aria-label="Nombre"
            className={inputClasses}
            required
          />
          {err('nombre') && <p className="mt-1 text-xs text-red-600">{err('nombre')}</p>}
        </div>
        <div>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
            aria-label="Email"
            className={inputClasses}
            required
          />
          {err('email') && <p className="mt-1 text-xs text-red-600">{err('email')}</p>}
        </div>
        <div>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Contraseña temporal (mín. 8 caracteres)"
            aria-label="Contraseña temporal"
            className={inputClasses}
            required
            minLength={8}
          />
          {err('password') && <p className="mt-1 text-xs text-red-600">{err('password')}</p>}
        </div>
        <select
          value={rol}
          onChange={(e) => setRol(e.target.value as RolUsuario)}
          aria-label="Rol"
          className={inputClasses}
        >
          <option value={RolUsuario.AsesorComercial}>Asesor Comercial</option>
          <option value={RolUsuario.Admin}>Admin</option>
        </select>

        <button
          type="submit"
          disabled={creando}
          className="font-heading col-span-full rounded-md bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
        >
          {creando ? 'Creando…' : 'Crear usuario'}
        </button>
      </form>
    </div>
  );
}
