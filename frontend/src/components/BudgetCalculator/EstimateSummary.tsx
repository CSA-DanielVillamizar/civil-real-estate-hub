import { CheckCircleIcon } from '../common/icons';
import type { EstimacionCosto } from '../../types/common';

// Nombres de presentación en español para las categorías que ya devuelve el
// backend tal cual (enum de Domain sin espacios) — sin inventar categorías
// nuevas (el mockup mostraba "Obra Gruesa/Instalaciones MEP/Acabados", que
// no existen en nuestro cálculo real).
const CATEGORIA_LABEL: Record<string, string> = {
  ManoDeObra: 'Mano de obra',
  Materiales: 'Materiales',
  Equipos: 'Equipos',
  AdministracionYUtilidad: 'Administración y utilidad',
};

function formatCurrency(monto: number, moneda: string): string {
  return new Intl.NumberFormat('es-CO', { style: 'currency', currency: moneda, maximumFractionDigits: 0 }).format(monto);
}

interface EstimateSummaryProps {
  estimacion: EstimacionCosto;
  // Opcional: con el área del formulario se puede mostrar el ratio $/m²
  // (mismo dato que muestra el mockup, pero calculado sobre el rango real
  // devuelto por el backend — no un número inventado).
  areaConstruccionM2?: number;
}

export function EstimateSummary({ estimacion, areaConstruccionM2 }: EstimateSummaryProps) {
  const promedio = (estimacion.montoMinimo + estimacion.montoMaximo) / 2;
  const totalDesglose = estimacion.desglose.reduce((acc, item) => acc + item.monto, 0);

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
      <p className="font-heading text-xs font-bold uppercase tracking-widest text-sky-600">Estimado de inversión</p>
      <p className="mt-1 font-heading text-2xl font-bold tracking-tight text-slate-900">
        {formatCurrency(estimacion.montoMinimo, estimacion.moneda)} – {formatCurrency(estimacion.montoMaximo, estimacion.moneda)}
      </p>

      {areaConstruccionM2 ? (
        <p className="mt-1 font-heading text-sm font-medium text-slate-500">
          Ratio unitario:{' '}
          <span className="font-semibold text-slate-700">{formatCurrency(promedio / areaConstruccionM2, estimacion.moneda)} / m²</span>
        </p>
      ) : null}

      <ul className="mt-4 flex flex-col gap-3 border-t border-slate-100 pt-4">
        {estimacion.desglose.map((item) => {
          const porcentaje = totalDesglose > 0 ? Math.round((item.monto / totalDesglose) * 100) : 0;
          return (
            <li key={item.categoria}>
              <div className="mb-1 flex items-center justify-between text-sm">
                <span className="font-medium text-slate-700">{CATEGORIA_LABEL[item.categoria] ?? item.categoria}</span>
                <span className="font-heading font-semibold text-slate-900">{formatCurrency(item.monto, estimacion.moneda)}</span>
              </div>
              <div className="h-1.5 w-full overflow-hidden rounded-full bg-slate-100">
                <div className="h-full rounded-full bg-sky-500" style={{ width: `${porcentaje}%` }} />
              </div>
            </li>
          );
        })}
      </ul>

      <p className="mt-4 flex items-start gap-2 text-xs text-slate-500">
        <CheckCircleIcon className="mt-0.5 h-3.5 w-3.5 shrink-0 text-sky-600" />
        Este valor es una estimación preliminar y puede variar según el diseño final, especificaciones técnicas y
        condiciones del terreno. Deja tus datos para que un asesor te contacte con una cotización detallada.
      </p>
    </div>
  );
}
