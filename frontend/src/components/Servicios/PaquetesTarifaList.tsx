import { usePaquetesTarifaPublicados } from '../../hooks/usePaquetesTarifaPublicados';
import type { ServicioDeInteres } from '../../types/common';
import type { PaqueteTarifa } from '../../types/tarifas';

function formatearPrecio(paquete: PaqueteTarifa): string {
  if (paquete.precioDesde == null && paquete.precioHasta == null) return 'Cotización personalizada';
  const fmt = (n: number) => n.toLocaleString('es-CO');
  if (paquete.precioDesde != null && paquete.precioHasta != null) {
    return `${paquete.moneda} ${fmt(paquete.precioDesde)} – ${fmt(paquete.precioHasta)} ${paquete.unidadPrecio}`;
  }
  const monto = paquete.precioDesde ?? paquete.precioHasta;
  return `Desde ${paquete.moneda} ${fmt(monto!)} ${paquete.unidadPrecio}`;
}

// Se muestra dentro de ConsultoriaEstructuralSection/InterventoriaSection —
// si no hay ningún paquete publicado para ese servicio, no renderiza nada
// (no tiene sentido un bloque de precios vacío).
export function PaquetesTarifaList({ servicio }: { servicio: ServicioDeInteres }) {
  const { items, isLoading, error } = usePaquetesTarifaPublicados();

  if (isLoading || error) return null;

  const paquetes = items.filter((item) => item.servicioRelacionado === servicio);
  if (paquetes.length === 0) return null;

  return (
    <div className="mt-6 flex flex-col gap-3 border-t border-slate-100 pt-5">
      <h3 className="font-heading text-xs font-bold uppercase tracking-wide text-slate-500">Precios de referencia</h3>
      {paquetes.map((paquete) => (
        <div
          key={paquete.id}
          className="rounded-xl border border-slate-200 bg-white p-4 shadow-[0_1px_3px_rgba(15,23,42,0.06)]"
        >
          <p className="font-heading text-sm font-semibold text-slate-900">{paquete.titulo}</p>
          <p className="font-heading mt-0.5 text-base font-bold text-emerald-700">{formatearPrecio(paquete)}</p>
          <p className="mt-1 text-xs text-slate-500">{paquete.descripcion}</p>
        </div>
      ))}
    </div>
  );
}
