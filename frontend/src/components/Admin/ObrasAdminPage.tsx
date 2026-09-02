import { useObrasAdmin } from '../../hooks/useObrasAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import type { ProyectoObraListItem } from '../../types/obras';
import { CrearProyectoObraForm } from './CrearProyectoObraForm';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const ESTADO_BADGE: Record<string, string> = {
  Planificacion: 'bg-slate-100 text-slate-700',
  EnEjecucion: 'bg-blue-100 text-blue-800',
  Pausado: 'bg-amber-100 text-amber-800',
  Finalizado: 'bg-emerald-100 text-emerald-800',
};

export function ObrasAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <Panel auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const { proyectos, isLoading, error, fieldErrors, crear } = useObrasAdmin(auth.token, onUnauthorized);

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-4xl px-6 py-10">
        <h1 className="mb-1 text-2xl font-bold text-slate-900">Avance de obra</h1>
        <p className="mb-6 text-sm text-slate-500">
          Crea un proyecto por cliente y comparte su link único — sin usuario ni contraseña, el cliente ve el
          avance con solo abrir el link.
        </p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        <div className="mb-8">
          <CrearProyectoObraForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : proyectos.length === 0 ? (
          <p className="text-sm text-slate-500">Aún no hay proyectos.</p>
        ) : (
          <div className="flex flex-col gap-3">
            {proyectos.map((p) => (
              <ProyectoRow key={p.id} proyecto={p} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function ProyectoRow({ proyecto }: { proyecto: ProyectoObraListItem }) {
  const link = `${window.location.origin}/mi-obra/${proyecto.tokenAcceso}`;

  return (
    <div className="flex items-center justify-between gap-4 rounded-lg border border-slate-200 bg-white p-4">
      <div>
        <p className="font-medium text-slate-900">{proyecto.nombreProyecto}</p>
        <p className="text-xs text-slate-500">
          {proyecto.nombreCliente} · {proyecto.hitosCompletados}/{proyecto.totalHitos} hitos completados
        </p>
      </div>

      <div className="flex items-center gap-2">
        <span className={`rounded-full px-2 py-1 text-xs font-medium ${ESTADO_BADGE[proyecto.estado] ?? 'bg-slate-100 text-slate-700'}`}>
          {proyecto.estado}
        </span>

        <button
          type="button"
          onClick={() => navigator.clipboard.writeText(link)}
          className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50"
        >
          Copiar link
        </button>

        <a
          href={`/admin/obras/${proyecto.id}`}
          className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700"
        >
          Gestionar
        </a>
      </div>
    </div>
  );
}
