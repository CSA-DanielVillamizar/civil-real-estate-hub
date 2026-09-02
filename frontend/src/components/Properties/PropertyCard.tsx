import type { PropertyResponse } from '../../types/properties';

const TIPO_INMUEBLE_LABEL: Record<string, string> = {
  Lote: 'Lote',
  Casa: 'Casa',
  Apartamento: 'Apartamento',
  Local: 'Local comercial',
  Bodega: 'Bodega',
  Finca: 'Finca',
};

export function PropertyCard({ property }: { property: PropertyResponse }) {
  return (
    <a
      href={`/propiedades/${property.id}`}
      className="group flex flex-col overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm transition hover:shadow-md"
    >
      <div className="aspect-[4/3] w-full overflow-hidden bg-slate-100">
        {property.fotoPrincipalUrl ? (
          <img
            src={property.fotoPrincipalUrl}
            alt={property.titulo}
            className="h-full w-full object-cover transition group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-sm text-slate-400">Sin foto</div>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-2 p-4">
        <div className="flex items-center justify-between gap-2">
          <span className="text-xs font-semibold uppercase tracking-wide text-emerald-700">
            {TIPO_INMUEBLE_LABEL[property.tipoInmueble] ?? property.tipoInmueble}
          </span>
          {property.esViableConstructivamente && (
            <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">
              Viable constructivamente
            </span>
          )}
        </div>

        <h3 className="font-semibold text-slate-900">{property.titulo}</h3>

        <p className="text-sm text-slate-500">
          {property.municipio}, {property.departamento}
        </p>

        <p className="text-sm text-slate-500">
          {property.areaTerrenoM2.toLocaleString('es-CO')} m²
          {property.areaConstruidaM2 ? ` · ${property.areaConstruidaM2.toLocaleString('es-CO')} m² construidos` : ''}
        </p>

        <p className="mt-auto text-lg font-bold text-slate-900">
          {property.precio.toLocaleString('es-CO')} {property.moneda}
        </p>
      </div>
    </a>
  );
}
