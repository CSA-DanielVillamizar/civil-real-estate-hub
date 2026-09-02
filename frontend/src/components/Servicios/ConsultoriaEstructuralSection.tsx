import { ServicioDeInteres } from '../../types/common';
import { ServicioInteresForm } from './ServicioInteresForm';

// Copy tomado de docs/02-business-case.md §3.2 — línea de negocio sin
// ninguna presencia digital previa (P1 de la auditoría interdisciplinaria).
export function ConsultoriaEstructuralSection() {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-6 sm:p-8">
      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        <div>
          <h2 className="text-2xl font-bold text-slate-900">Consultoría y Diseño Estructural</h2>
          <p className="mt-2 text-slate-500">
            Adaptamos el diseño a la compleja topografía antioqueña, sin afectar el entorno.
          </p>
          <ul className="mt-5 flex flex-col gap-4 text-sm text-slate-600">
            <li>
              <span className="font-semibold text-slate-900">Bioarquitectura modular</span> — diseños adaptados al
              terreno, como viviendas de niveles escalonados que respetan la pendiente natural del lote.
            </li>
            <li>
              <span className="font-semibold text-slate-900">Sistemas constructivos livianos</span> —
              especificación y consultoría en Light Gauge Steel Framing (exoesqueletos de acero) para optimizar
              tiempos de obra y reducir la carga muerta en terrenos inclinados.
            </li>
          </ul>
        </div>

        <div>
          <ServicioInteresForm servicio={ServicioDeInteres.ConsultoriaYDisenoEstructural} />
        </div>
      </div>
    </section>
  );
}
