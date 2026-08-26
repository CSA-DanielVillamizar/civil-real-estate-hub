import type { EstimacionCosto } from '../../types/common';

function formatCurrency(monto: number, moneda: string): string {
  return new Intl.NumberFormat('es-CO', { style: 'currency', currency: moneda, maximumFractionDigits: 0 }).format(monto);
}

interface EstimateSummaryProps {
  estimacion: EstimacionCosto;
}

export function EstimateSummary({ estimacion }: EstimateSummaryProps) {
  return (
    <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-5">
      <p className="text-sm font-medium text-emerald-800">Estimado de inversión</p>
      <p className="mt-1 text-2xl font-bold text-emerald-900">
        {formatCurrency(estimacion.montoMinimo, estimacion.moneda)} – {formatCurrency(estimacion.montoMaximo, estimacion.moneda)}
      </p>

      <ul className="mt-4 space-y-2 border-t border-emerald-200 pt-4">
        {estimacion.desglose.map((item) => (
          <li key={item.categoria} className="flex items-center justify-between text-sm">
            <span className="text-emerald-800">{item.categoria}</span>
            <span className="font-medium text-emerald-900">{formatCurrency(item.monto, estimacion.moneda)}</span>
          </li>
        ))}
      </ul>

      <p className="mt-4 text-xs text-emerald-700">
        Este valor es una estimación preliminar y puede variar según el diseño final, especificaciones técnicas y
        condiciones del terreno. Deja tus datos para que un asesor te contacte con una cotización detallada.
      </p>
    </div>
  );
}
