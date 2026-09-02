import { TipoInmueble } from '../../types/common';
import type { GetPropertiesParams } from '../../types/properties';

interface PropertyFiltersProps {
  values: GetPropertiesParams;
  onChange: (values: GetPropertiesParams) => void;
}

const inputClasses =
  'w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 shadow-sm outline-none transition focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

export function PropertyFilters({ values, onChange }: PropertyFiltersProps) {
  function set<K extends keyof GetPropertiesParams>(field: K, value: GetPropertiesParams[K]) {
    onChange({ ...values, [field]: value, page: 1 });
  }

  return (
    <div className="mb-6 grid grid-cols-2 gap-3 rounded-xl border border-slate-200 bg-white p-4 shadow-sm sm:grid-cols-3 lg:grid-cols-6">
      <select
        aria-label="Tipo de inmueble"
        value={values.tipoInmueble ?? ''}
        onChange={(e) => set('tipoInmueble', (e.target.value || undefined) as GetPropertiesParams['tipoInmueble'])}
        className={inputClasses}
      >
        <option value="">Todos los tipos</option>
        {Object.values(TipoInmueble).map((tipo) => (
          <option key={tipo} value={tipo}>
            {tipo}
          </option>
        ))}
      </select>

      <input
        aria-label="Municipio"
        type="text"
        placeholder="Municipio"
        value={values.municipio ?? ''}
        onChange={(e) => set('municipio', e.target.value || undefined)}
        className={inputClasses}
      />

      <input
        aria-label="Precio mínimo"
        type="number"
        placeholder="Precio mín."
        value={values.precioMin ?? ''}
        onChange={(e) => set('precioMin', e.target.value ? Number(e.target.value) : undefined)}
        className={inputClasses}
      />

      <input
        aria-label="Precio máximo"
        type="number"
        placeholder="Precio máx."
        value={values.precioMax ?? ''}
        onChange={(e) => set('precioMax', e.target.value ? Number(e.target.value) : undefined)}
        className={inputClasses}
      />

      <input
        aria-label="Área mínima"
        type="number"
        placeholder="Área mín. (m²)"
        value={values.areaMin ?? ''}
        onChange={(e) => set('areaMin', e.target.value ? Number(e.target.value) : undefined)}
        className={inputClasses}
      />

      <label className="flex items-center gap-2 text-sm text-slate-600">
        <input
          type="checkbox"
          checked={values.soloViablesConstructivamente ?? false}
          onChange={(e) => set('soloViablesConstructivamente', e.target.checked || undefined)}
          className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500"
        />
        Solo viables
      </label>
    </div>
  );
}
