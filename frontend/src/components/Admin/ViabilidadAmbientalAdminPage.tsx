import type { AuthState } from '../../hooks/useAuth';
import { useSolicitudesViabilidadAmbiental } from '../../hooks/useSolicitudesViabilidadAmbiental';
import { RolUsuario } from '../../types/auth';
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
      <h1 className="mb-1 text-2xl font-bold text-slate-900">Solicitudes de viabilidad ambiental</h1>
      <p className="mb-6 text-sm text-slate-500">
        Confirma el pago tras verificar la transferencia en la cuenta bancaria publicada.
      </p>

      {error && (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>
      )}

      {isLoading ? (
        <p className="text-sm text-slate-500">Cargando…</p>
      ) : solicitudes.length === 0 ? (
        <p className="text-sm text-slate-500">No hay solicitudes registradas.</p>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-slate-200">
          <table className="min-w-full divide-y divide-slate-200 text-sm">
            <thead className="bg-slate-50 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              <tr>
                <th className="px-4 py-3">Solicitante</th>
                <th className="px-4 py-3">Ubicación</th>
                <th className="px-4 py-3">Monto</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3">Solicitada</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {solicitudes.map((s) => (
                <tr key={s.id}>
                  <td className="px-4 py-3">
                    <div className="font-medium text-slate-900">{s.nombre}</div>
                    <div className="text-xs text-slate-500">
                      {s.email} · {s.telefono}
                    </div>
                  </td>
                  <td className="px-4 py-3 text-slate-600">
                    {s.propiedadId ? `Propiedad ${s.propiedadId.slice(0, 8)}…` : `${s.municipio}, ${s.departamento}`}
                  </td>
                  <td className="px-4 py-3 text-slate-900">
                    {s.monto.toLocaleString('es-CO')} {s.moneda}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`rounded-full px-2 py-1 text-xs font-medium ${ESTADO_BADGE[s.estado] ?? 'bg-slate-100 text-slate-700'}`}>
                      {s.estado}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-slate-500">{new Date(s.solicitadaEn).toLocaleString('es-CO')}</td>
                  <td className="px-4 py-3 text-right">
                    {s.estado === 'Solicitada' && (
                      <button
                        onClick={() => confirmarPago(s.id)}
                        disabled={confirmandoId === s.id}
                        className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
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
    </div>
    </div>
  );
}
