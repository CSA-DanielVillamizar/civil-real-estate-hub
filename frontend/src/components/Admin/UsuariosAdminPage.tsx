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
        <h1 className="mb-1 text-2xl font-bold text-slate-900">Usuarios del equipo</h1>
        <p className="mb-6 text-sm text-slate-500">
          Crea cuentas para tus asesores comerciales o para otros administradores. Los Asesores Comerciales solo ven
          el panel de Leads.
        </p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        <div className="mb-8">
          <CrearUsuarioForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : usuarios.length === 0 ? (
          <p className="text-sm text-slate-500">Aún no hay usuarios (raro — al menos tu cuenta debería aparecer aquí).</p>
        ) : (
          <div className="flex flex-col gap-3">
            {usuarios.map((u) => (
              <UsuarioRow key={u.id} usuario={u} busy={busyId === u.id} onCambiarActivo={cambiarActivo} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function UsuarioRow({
  usuario,
  busy,
  onCambiarActivo,
}: {
  usuario: UsuarioListItem;
  busy: boolean;
  onCambiarActivo: (id: string, activo: boolean) => Promise<void>;
}) {
  return (
    <div className="flex items-center justify-between gap-4 rounded-lg border border-slate-200 bg-white p-4">
      <div>
        <p className="font-medium text-slate-900">{usuario.nombre}</p>
        <p className="text-xs text-slate-500">
          {usuario.email} · {usuario.rol}
        </p>
      </div>

      <div className="flex items-center gap-2">
        <span
          className={`rounded-full px-2 py-1 text-xs font-medium ${
            usuario.activo ? 'bg-emerald-100 text-emerald-800' : 'bg-slate-100 text-slate-500'
          }`}
        >
          {usuario.activo ? 'Activo' : 'Inactivo'}
        </span>

        <button
          type="button"
          onClick={() => onCambiarActivo(usuario.id, !usuario.activo)}
          disabled={busy}
          className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          {busy ? '…' : usuario.activo ? 'Desactivar' : 'Activar'}
        </button>
      </div>
    </div>
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
      <h3 className="mb-3 font-semibold text-slate-900">Nuevo usuario</h3>

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
          className="col-span-full rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
        >
          {creando ? 'Creando…' : 'Crear usuario'}
        </button>
      </form>
    </div>
  );
}
