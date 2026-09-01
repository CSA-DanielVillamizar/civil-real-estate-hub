import { useSolicitarViabilidadAmbiental } from '../../hooks/useSolicitarViabilidadAmbiental';
import { InstruccionesPago } from './InstruccionesPago';
import { ViabilidadAmbientalForm } from './ViabilidadAmbientalForm';

export function ViabilidadAmbientalSection() {
  const { isSubmitting, error, fieldErrors, resultado, solicitar, reset } = useSolicitarViabilidadAmbiental();

  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm sm:p-8">
      <div className="mb-6">
        <h2 className="text-xl font-bold text-slate-900">Estudio de viabilidad ambiental</h2>
        <p className="mt-1 text-sm text-slate-500">Retiros hídricos y pendiente del terreno, revisados por un técnico.</p>
      </div>

      {error && !resultado && (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>
      )}

      {resultado ? (
        <InstruccionesPago resultado={resultado} onNuevaSolicitud={reset} />
      ) : (
        <ViabilidadAmbientalForm isSubmitting={isSubmitting} serverFieldErrors={fieldErrors} onSubmit={solicitar} />
      )}
    </section>
  );
}
