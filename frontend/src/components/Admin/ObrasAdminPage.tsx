import { useObrasAdmin } from '../../hooks/useObrasAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import { EstadoProyecto, type ProyectoObraListItem } from '../../types/obras';
import { CrearProyectoObraForm } from './CrearProyectoObraForm';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const ESTADO_BADGE: Record<string, string> = {
  Planificacion: 'bg-slate-100 text-slate-700',
  EnEjecucion: 'bg-sky-100 text-sky-800',
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
      <div className="mx-auto max-w-5xl px-6 py-10">
        <h1 className="font-heading mb-1 text-2xl font-bold tracking-tight text-slate-900">Avance de obra</h1>
        <p className="mb-6 text-sm text-slate-500">
          Crea un proyecto por cliente y comparte su link único — sin usuario ni contraseña, el cliente ve el
          avance con solo abrir el link.
        </p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        {!isLoading && proyectos.length > 0 && <ResumenProyectos proyectos={proyectos} />}

        <div className="mb-8">
          <CrearProyectoObraForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : proyectos.length === 0 ? (
          <p className="text-sm text-slate-500">Aún no hay proyectos.</p>
        ) : (
          <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
            {proyectos.map((p, i) => (
              <ProyectoRow key={p.id} proyecto={p} esUltimo={i === proyectos.length - 1} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

// Tarjetas de resumen — solo agregados reales calculados sobre los
// proyectos ya cargados (conteos y suma de hitos), nada inventado como en
// el mockup ("$4.2M en licitaciones", "98.4% viabilidad estructural").
function ResumenProyectos({ proyectos }: { proyectos: ProyectoObraListItem[] }) {
  const enEjecucion = proyectos.filter((p) => p.estado === EstadoProyecto.EnEjecucion).length;
  const finalizados = proyectos.filter((p) => p.estado === EstadoProyecto.Finalizado).length;
  const hitosCompletados = proyectos.reduce((acc, p) => acc + p.hitosCompletados, 0);
  const totalHitos = proyectos.reduce((acc, p) => acc + p.totalHitos, 0);

  const tarjetas = [
    { etiqueta: 'Proyectos activos', valor: proyectos.length },
    { etiqueta: 'En ejecución', valor: enEjecucion },
    { etiqueta: 'Finalizados', valor: finalizados },
    { etiqueta: 'Hitos completados', valor: `${hitosCompletados}/${totalHitos}` },
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

function ProyectoRow({ proyecto, esUltimo }: { proyecto: ProyectoObraListItem; esUltimo: boolean }) {
  const link = `${window.location.origin}/mi-obra/${proyecto.tokenAcceso}`;
  const progreso = proyecto.totalHitos > 0 ? Math.round((proyecto.hitosCompletados / proyecto.totalHitos) * 100) : 0;

  return (
    <div className={`flex flex-col gap-3 p-4 sm:flex-row sm:items-center sm:justify-between ${esUltimo ? '' : 'border-b border-slate-100'}`}>
      <div className="min-w-0 flex-1">
        <p className="font-heading truncate font-semibold text-slate-900">{proyecto.nombreProyecto}</p>
        <p className="text-xs text-slate-500">{proyecto.nombreCliente}</p>
        <div className="mt-2 flex items-center gap-2">
          <div className="h-1.5 w-32 overflow-hidden rounded-full bg-slate-100">
            <div className="h-full rounded-full bg-sky-500" style={{ width: `${progreso}%` }} />
          </div>
          <span className="font-heading text-xs font-medium text-slate-500">
            {proyecto.hitosCompletados}/{proyecto.totalHitos} hitos completados
          </span>
        </div>
      </div>

      <div className="flex shrink-0 items-center gap-2">
        <span className={`font-heading rounded-full px-2 py-1 text-[11px] font-bold uppercase tracking-wide ${ESTADO_BADGE[proyecto.estado] ?? 'bg-slate-100 text-slate-700'}`}>
          {proyecto.estado}
        </span>

        <button
          type="button"
          onClick={() => navigator.clipboard.writeText(link)}
          className="font-heading rounded-md border border-slate-300 px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-50"
        >
          Copiar link
        </button>

        <a
          href={`/admin/obras/${proyecto.id}`}
          className="font-heading rounded-md bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white hover:bg-slate-800"
        >
          Gestionar
        </a>
      </div>
    </div>
  );
}
