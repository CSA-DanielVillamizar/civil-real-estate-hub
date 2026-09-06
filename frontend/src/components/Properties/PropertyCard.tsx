import { AlertTriangleIcon, CheckCircleIcon, LocationPinIcon } from '../common/icons';
import { WhatsAppButton } from './WhatsAppButton';
import { TIPO_INMUEBLE_LABEL } from './propertyLabels';
import type { PropertyResponse } from '../../types/properties';

interface PropertyCardProps {
  property: PropertyResponse;
  // El checkbox de selección para el comparador es opcional — PropertyCard
  // se sigue usando tal cual en cualquier otro lugar que no lo necesite.
  seleccionable?: boolean;
  seleccionado?: boolean;
  onToggleSeleccion?: (id: string) => void;
}

export function PropertyCard({ property, seleccionable, seleccionado, onToggleSeleccion }: PropertyCardProps) {
  return (
    <div className="flex flex-col overflow-hidden rounded-xl bg-white shadow-[0_1px_3px_rgba(15,23,42,0.06)] ring-1 ring-slate-200 transition-transform duration-300 hover:-translate-y-1 hover:shadow-lg">
      {/* Toda la superficie navegable vive en el <a>; el checkbox de abajo es
          hermano suyo, no anidado dentro — un input dentro de un enlace es
          HTML inválido y además complica evitar que el clic navegue a la
          ficha en vez de solo marcar la casilla. */}
      <a href={`/propiedades/${property.id}`} className="group flex flex-col">
        <div className="relative aspect-[4/3] w-full overflow-hidden bg-slate-100">
          {property.fotoPrincipalUrl ? (
            <img
              src={property.fotoPrincipalUrl}
              alt={property.titulo}
              className="h-full w-full object-cover transition duration-500 group-hover:scale-105"
            />
          ) : (
            <div className="flex h-full w-full items-center justify-center text-sm text-slate-400">Sin foto</div>
          )}

          {/* Insignias reales del inmueble — tipo y viabilidad constructiva
              calculada, nunca certificaciones o datos que no tenemos. */}
          <div className="absolute left-3 top-3 flex flex-col items-start gap-1.5">
            <span className="rounded bg-slate-900/90 px-2 py-1 font-heading text-[11px] font-bold uppercase tracking-wider text-white backdrop-blur">
              {TIPO_INMUEBLE_LABEL[property.tipoInmueble] ?? property.tipoInmueble}
            </span>
            {property.esViableConstructivamente ? (
              <span className="flex items-center gap-1 rounded bg-white/90 px-2 py-1 font-heading text-[11px] font-bold uppercase tracking-wider text-emerald-700 backdrop-blur">
                <CheckCircleIcon className="h-3 w-3" /> Viable constructivamente
              </span>
            ) : (
              <span className="flex items-center gap-1 rounded bg-white/90 px-2 py-1 font-heading text-[11px] font-bold uppercase tracking-wider text-amber-700 backdrop-blur">
                <AlertTriangleIcon className="h-3 w-3" /> Con restricciones
              </span>
            )}
          </div>
        </div>

        <div className="flex flex-col gap-1.5 p-4">
          <div className="flex items-center gap-1 font-heading text-xs font-medium text-slate-500">
            <LocationPinIcon className="h-3.5 w-3.5 text-sky-600" />
            <span>
              {property.municipio}, {property.departamento}
            </span>
          </div>

          <h3 className="font-heading text-lg font-semibold leading-snug text-slate-900">{property.titulo}</h3>

          <p className="font-heading text-2xl font-bold tracking-tight text-slate-900">
            {property.precio.toLocaleString('es-CO')}{' '}
            <span className="text-base font-semibold text-slate-500">{property.moneda}</span>
          </p>

          <div className="mt-1 grid grid-cols-2 gap-2 rounded-lg bg-slate-50 p-2.5 text-center">
            <div className="flex flex-col">
              <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Terreno</span>
              <span className="font-heading text-sm font-semibold text-slate-900">
                {property.areaTerrenoM2.toLocaleString('es-CO')} m²
              </span>
            </div>
            <div className="flex flex-col">
              <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Construido</span>
              <span className="font-heading text-sm font-semibold text-slate-900">
                {property.areaConstruidaM2 ? `${property.areaConstruidaM2.toLocaleString('es-CO')} m²` : '—'}
              </span>
            </div>
          </div>
        </div>
      </a>

      <div className="flex flex-col gap-2.5 px-4 pb-4">
        {seleccionable && (
          <label className="flex cursor-pointer select-none items-center gap-2 text-sm text-slate-600">
            <input
              type="checkbox"
              checked={seleccionado ?? false}
              onChange={() => onToggleSeleccion?.(property.id)}
              className="h-4 w-4 rounded border-slate-300 text-sky-600 focus:ring-sky-500"
            />
            Seleccionar para comparar
          </label>
        )}
        <div className="grid grid-cols-2 gap-2">
          <a
            href={`/propiedades/${property.id}`}
            className="flex items-center justify-center rounded-lg border border-slate-200 px-3 py-2 font-heading text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
          >
            Ver detalle
          </a>
          <WhatsAppButton
            mensaje={`Hola, me interesa la propiedad "${property.titulo}" (${property.municipio}, ${property.departamento}).`}
          />
        </div>
      </div>
    </div>
  );
}
