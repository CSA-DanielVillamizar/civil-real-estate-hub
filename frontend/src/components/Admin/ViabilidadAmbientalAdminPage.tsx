import { useState, type FormEvent } from 'react';
import { useAdminApiKey } from '../../hooks/useAdminApiKey';
import { useSolicitudesViabilidadAmbiental } from '../../hooks/useSolicitudesViabilidadAmbiental';
import { AdminNav } from './AdminNav';

const ESTADO_BADGE: Record<string, string> = {
  Solicitada: 'bg-amber-100 text-amber-800',
  Pagada: 'bg-emerald-100 text-emerald-800',
  Rechazada: 'bg-red-100 text-red-800',
};

export function ViabilidadAmbientalAdminPage() {
  const { apiKey, guardar, limpiar } = useAdminApiKey();

  if (!apiKey) {
    return <ApiKeyGate onGuardar={guardar} />;
  }

  return <PanelSolicitudes apiKey={apiKey} onUnauthorized={limpiar} />;
}

function ApiKeyGate({ onGuardar }: { onGuardar: (apiKey: string) => void }) {
  const [valor, setValor] = useState('');

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (valor.trim()) onGuardar(valor.trim());
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-sm flex-col justify-center px-6">
      <h1 className="mb-2 text-xl font-bold text-slate-900">Panel administrativo</h1>
      <p className="mb-6 text-sm text-slate-500">
        Ingresa el API key de administrador. Queda guardado solo en este navegador.
      </p>
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <input
          type="password"
          value={valor}
          onChange={(e) => setValor(e.target.value)}
          placeholder="X-Admin-Api-Key"
          className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none"
          autoFocus
        />
        <button
          type="submit"
          disabled={!valor.trim()}
          className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
        >
          Entrar
        </button>
      </form>
    </div>
  );
}

function PanelSolicitudes({ apiKey, onUnauthorized }: { apiKey: string; onUnauthorized: () => void }) {
  const { solicitudes, isLoading, error, confirmandoId, confirmarPago } = useSolicitudesViabilidadAmbiental(
    apiKey,
    onUnauthorized,
  );

  return (
    <div>
    <AdminNav />
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
