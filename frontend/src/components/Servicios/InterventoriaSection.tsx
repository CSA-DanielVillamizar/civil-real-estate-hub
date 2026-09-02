import { ServicioDeInteres } from '../../types/common';
import { ServicioInteresForm } from './ServicioInteresForm';

// Copy tomado de docs/02-business-case.md §3.3 — línea de negocio sin
// ninguna presencia digital previa (P1 de la auditoría interdisciplinaria).
export function InterventoriaSection() {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8">
      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        <div>
          <h2 className="text-2xl font-bold text-slate-900">Interventoría y Presupuestos</h2>
          <p className="mt-2 text-slate-500">
            Auditoría integral para proyectos de terceros o propios, del presupuesto a la ejecución.
          </p>
          <ul className="mt-5 flex flex-col gap-4 text-sm text-slate-600">
            <li>
              <span className="font-semibold text-slate-900">Revisión de presupuestos</span> — optimizamos el
              presupuesto de obra para evitar sobrecostos antes de que empiece la construcción.
            </li>
            <li>
              <span className="font-semibold text-slate-900">Interventoría técnica</span> — aseguramos el
              cumplimiento de las normativas estructurales y ambientales durante toda la ejecución del proyecto.
            </li>
          </ul>
        </div>

        <div>
          <ServicioInteresForm servicio={ServicioDeInteres.InterventoriaYPresupuestos} />
        </div>
      </div>
    </section>
  );
}
