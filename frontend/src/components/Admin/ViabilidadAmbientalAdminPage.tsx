import type { AuthState } from '../../hooks/useAuth';
import { useSolicitudesViabilidadAmbiental } from '../../hooks/useSolicitudesViabilidadAmbiental';
import { RolUsuario } from '../../types/auth';
import type { SolicitudViabilidadAmbientalListItem } from '../../types/viabilidadAmbiental';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const ESTADO_BADGE: Record<string, string> = {
  Solicitada: 'bg-amber-100 text-amber-800',
  Pagada: 'bg-emerald-100 text-emerald-800',
  Rechazada: 'bg-red-100 text-red-800',
};

export function ViabilidadAmbientalAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <PanelSolicitudes auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function PanelSolicitudes({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const { solicitudes, isLoading, error, confirmandoId, confirmarPago } = useSolicitudesViabilidadAmbiental(
    auth.token,
    onUnauthorized,
  );

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-5xl px-6 py-10">
        <h1 className="font-heading mb-1 text-2xl font-bold tracking-tight text-slate-900">Solicitudes de viabilidad ambiental</h1>
        <p className="mb-6 text-sm text-slate-500">
          Confirma el pago tras verificar la transferencia en la cuenta bancaria publicada.
        </p>

        {error && (
          <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>
        )}

        {!isLoading && solicitudes.length > 0 && <ResumenSolicitudes solicitudes={solicitudes} />}

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : solicitudes.length === 0 ? (
          <p className="text-sm text-slate-500">No hay solicitudes registradas.</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
            <table className="w-full min-w-[760px] table-fixed border-collapse text-left text-sm">
              <thead className="bg-slate-50 text-left text-[11px] font-bold uppercase tracking-wide text-slate-500">
                <tr>
                  <th className="w-56 px-3 py-2">Solicitante</th>
                  <th className="w-44 px-3 py-2">Predio</th>
                  <th className="w-32 px-3 py-2">Monto</th>
                  <th className="w-28 px-3 py-2">Estado</th>
                  <th className="w-36 px-3 py-2">Solicitada</th>
                  <th className="px-3 py-2 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {solicitudes.map((s) => (
                  <tr key={s.id} className="align-top hover:bg-slate-50">
                    <td className="px-3 py-2">
                      <p className="font-heading font-semibold text-slate-900">{s.nombre}</p>
                      <p className="text-xs text-slate-500">
                        {s.email} · {s.telefono}
                      </p>
                    </td>
                    <td className="px-3 py-2 text-slate-600">
                      {s.propiedadId ? `Propiedad ${s.propiedadId.slice(0, 8)}…` : `${s.municipio}, ${s.departamento}`}
                    </td>
                    <td className="font-heading px-3 py-2 font-medium text-slate-900">
                      {s.monto.toLocaleString('es-CO')} {s.moneda}
                    </td>
                    <td className="px-3 py-2">
                      <span className={`font-heading rounded-full px-2 py-1 text-[11px] font-bold uppercase tracking-wide ${ESTADO_BADGE[s.estado] ?? 'bg-slate-100 text-slate-700'}`}>
                        {s.estado}
                      </span>
                    </td>
                    <td className="px-3 py-2 text-xs text-slate-500">{new Date(s.solicitadaEn).toLocaleString('es-CO')}</td>
                    <td className="px-3 py-2 text-right">
                      {s.estado === 'Solicitada' && (
                        <button
                          onClick={() => confirmarPago(s.id)}
                          disabled={confirmandoId === s.id}
                          className="font-heading rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
                        >
                          {confirmandoId === s.id ? 'Confirmando…' : 'Confirmar pago'}
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Nota honesta: el dictamen técnico (tipo de suelo, retiros
            normativos, restricciones) es un entregable externo del estudio
            pagado — esta lista es el registro de la solicitud/pago, no
            contiene esos datos técnicos. Cuando la solicitud referencia un
            predio ya catalogado (propiedadId), esa ficha sí tiene la
            viabilidad calculada — ver /admin/propiedades. */}
      </div>
    </div>
  );
}

// Tarjetas de resumen — agregados 100% reales sobre las solicitudes ya
// cargadas, para evaluar la carga de trabajo pendiente en segundos.
function ResumenSolicitudes({ solicitudes }: { solicitudes: SolicitudViabilidadAmbientalListItem[] }) {
  const pendientes = solicitudes.filter((s) => s.estado === 'Solicitada').length;
  const pagadas = solicitudes.filter((s) => s.estado === 'Pagada').length;
  const rechazadas = solicitudes.filter((s) => s.estado === 'Rechazada').length;

  const tarjetas = [
    { etiqueta: 'Total solicitudes', valor: solicitudes.length },
    { etiqueta: 'Pendientes de pago', valor: pendientes },
    { etiqueta: 'Pagadas', valor: pagadas },
    { etiqueta: 'Rechazadas', valor: rechazadas },
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
