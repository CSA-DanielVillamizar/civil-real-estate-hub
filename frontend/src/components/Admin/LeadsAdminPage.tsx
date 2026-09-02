import { useLeadsAdmin } from '../../hooks/useLeadsAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { EstadoLead } from '../../types/common';
import { RolUsuario } from '../../types/auth';
import type { LeadListItem } from '../../types/leads';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const ESTADO_BADGE: Record<string, string> = {
  Nuevo: 'bg-slate-100 text-slate-700',
  Contactado: 'bg-blue-100 text-blue-800',
  Calificado: 'bg-amber-100 text-amber-800',
  Convertido: 'bg-emerald-100 text-emerald-800',
  Descartado: 'bg-red-100 text-red-800',
  ContactoPendientePorReasignacion: 'bg-purple-100 text-purple-800',
};

const ORIGEN_LABEL: Record<string, string> = {
  CalculadoraObra: 'Calculadora de obra',
  FormularioContacto: 'Formulario de contacto',
  LandingPage: 'Landing page',
  Referido: 'Referido',
};

const SERVICIO_LABEL: Record<string, string> = {
  Inmobiliaria: 'Inmobiliaria',
  CalculadoraDeObra: 'Calculadora de obra',
  ConsultoriaYDisenoEstructural: 'Consultoría estructural',
  InterventoriaYPresupuestos: 'Interventoría y presupuestos',
};

export function LeadsAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin, RolUsuario.AsesorComercial]}>
      {(auth, onUnauthorized) => <Panel auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const { leads, isLoading, error, busyId, filtro, setFiltro, marcarContactado, calificar, convertir, descartar } =
    useLeadsAdmin(auth.token, onUnauthorized);

  function handleDescartar(lead: LeadListItem) {
    const motivo = window.prompt(`¿Por qué se descarta a ${lead.nombre}?`);
    if (motivo && motivo.trim()) descartar(lead.id, motivo.trim());
  }

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-6xl px-6 py-10">
      <h1 className="mb-1 text-2xl font-bold text-slate-900">Leads</h1>
      <p className="mb-6 text-sm text-slate-500">
        Da seguimiento a cada lead: márcalo contactado, califícalo, conviértelo en cliente o descártalo.
      </p>

      <div className="mb-5 flex items-center gap-3">
        <label htmlFor="filtro-estado" className="text-sm font-medium text-slate-700">
          Estado
        </label>
        <select
          id="filtro-estado"
          value={filtro}
          onChange={(e) => setFiltro(e.target.value as EstadoLead | '')}
          className="rounded-md border border-slate-300 px-3 py-1.5 text-sm shadow-sm focus:border-emerald-500 focus:outline-none"
        >
          <option value="">Todos</option>
          {Object.values(EstadoLead).map((estado) => (
            <option key={estado} value={estado}>
              {estado}
            </option>
          ))}
        </select>
      </div>

      {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      {isLoading ? (
        <p className="text-sm text-slate-500">Cargando…</p>
      ) : leads.length === 0 ? (
        <p className="text-sm text-slate-500">No hay leads que coincidan con este filtro.</p>
      ) : (
        <div className="overflow-x-auto rounded-lg border border-slate-200">
          <table className="min-w-full divide-y divide-slate-200 text-sm">
            <thead className="bg-slate-50 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              <tr>
                <th className="px-4 py-3">Lead</th>
                <th className="px-4 py-3">Servicio de interés</th>
                <th className="px-4 py-3">Origen</th>
                <th className="px-4 py-3">Estimación</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3">Capturado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {leads.map((lead) => (
                <tr key={lead.id}>
                  <td className="px-4 py-3">
                    <div className="font-medium text-slate-900">{lead.nombre}</div>
                    <div className="text-xs text-slate-500">
                      {lead.email} · {lead.telefono}
                    </div>
                  </td>
                  <td className="px-4 py-3 text-slate-600" title={lead.mensaje}>
                    {lead.servicioDeInteres ? (SERVICIO_LABEL[lead.servicioDeInteres] ?? lead.servicioDeInteres) : '—'}
                    {lead.mensaje && <span className="ml-1 text-slate-400">💬</span>}
                  </td>
                  <td className="px-4 py-3 text-slate-600">{ORIGEN_LABEL[lead.origen] ?? lead.origen}</td>
                  <td className="px-4 py-3 text-slate-600">
                    {lead.estimacionMontoMinimo != null
                      ? `${lead.estimacionMontoMinimo.toLocaleString('es-CO')}–${lead.estimacionMontoMaximo!.toLocaleString('es-CO')} ${lead.estimacionMoneda}`
                      : '—'}
                  </td>
                  <td className="px-4 py-3">
                    <span className={`rounded-full px-2 py-1 text-xs font-medium ${ESTADO_BADGE[lead.estado] ?? 'bg-slate-100 text-slate-700'}`}>
                      {lead.estado}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-slate-500">{new Date(lead.capturadoEn).toLocaleString('es-CO')}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-2 whitespace-nowrap">
                      {lead.estado === 'Nuevo' && (
                        <button
                          onClick={() => marcarContactado(lead.id)}
                          disabled={busyId === lead.id}
                          className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                        >
                          Marcar contactado
                        </button>
                      )}
                      {lead.estado === 'Contactado' && (
                        <button
                          onClick={() => calificar(lead.id)}
                          disabled={busyId === lead.id}
                          className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                        >
                          Calificar
                        </button>
                      )}
                      {lead.estado === 'Calificado' && (
                        <button
                          onClick={() => convertir(lead.id)}
                          disabled={busyId === lead.id}
                          className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
                        >
                          Convertir
                        </button>
                      )}
                      {lead.estado !== 'Convertido' && lead.estado !== 'Descartado' && (
                        <button
                          onClick={() => handleDescartar(lead)}
                          disabled={busyId === lead.id}
                          className="rounded-md border border-red-200 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
                        >
                          Descartar
                        </button>
                      )}
                    </div>
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
