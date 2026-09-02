import { useState } from 'react';
import { useProperties } from '../../hooks/useProperties';
import type { GetPropertiesParams } from '../../types/properties';
import { PropertyCard } from './PropertyCard';
import { PropertyFilters } from './PropertyFilters';

const initialFilters: GetPropertiesParams = { page: 1, pageSize: 12 };
const MAX_COMPARACION = 4;

export function PropertiesSection() {
  const [filters, setFilters] = useState<GetPropertiesParams>(initialFilters);
  const [seleccionadas, setSeleccionadas] = useState<string[]>([]);
  const { data, isLoading, error } = useProperties(filters);

  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / data.pageSize)) : 1;

  // Orden fijo (no un Set): el orden en que se seleccionan las propiedades
  // se conserva en el comparador — más predecible que el orden de un Set.
  function toggleSeleccion(id: string) {
    setSeleccionadas((prev) => {
      if (prev.includes(id)) return prev.filter((p) => p !== id);
      if (prev.length >= MAX_COMPARACION) return prev;
      return [...prev, id];
    });
  }

  return (
    <section>
      <div className="mb-6 text-center">
        <h2 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">Propiedades disponibles</h2>
        <p className="mx-auto mt-2 max-w-2xl text-slate-500">
          Cada ficha incluye un pre-análisis de viabilidad constructiva — retiros ambientales y pendiente del terreno.
        </p>
      </div>

      <PropertyFilters values={filters} onChange={setFilters} />

      {error && (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>
      )}

      {isLoading ? (
        <p className="text-center text-sm text-slate-500">Cargando propiedades…</p>
      ) : !data || data.items.length === 0 ? (
        <p className="text-center text-sm text-slate-500">No hay propiedades que coincidan con estos filtros.</p>
      ) : (
        <>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {data.items.map((property) => (
              <PropertyCard
                key={property.id}
                property={property}
                seleccionable
                seleccionado={seleccionadas.includes(property.id)}
                onToggleSeleccion={toggleSeleccion}
              />
            ))}
          </div>

          {totalPages > 1 && (
            <div className="mt-8 flex items-center justify-center gap-4">
              <button
                type="button"
                disabled={(filters.page ?? 1) <= 1}
                onClick={() => setFilters((prev) => ({ ...prev, page: (prev.page ?? 1) - 1 }))}
                className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 disabled:opacity-40"
              >
                Anterior
              </button>
              <span className="text-sm text-slate-500">
                Página {filters.page ?? 1} de {totalPages}
              </span>
              <button
                type="button"
                disabled={(filters.page ?? 1) >= totalPages}
                onClick={() => setFilters((prev) => ({ ...prev, page: (prev.page ?? 1) + 1 }))}
                className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 disabled:opacity-40"
              >
                Siguiente
              </button>
            </div>
          )}
        </>
      )}

      {seleccionadas.length > 0 && (
        <div className="fixed inset-x-0 bottom-0 z-40 border-t border-slate-200 bg-white px-6 py-3 shadow-[0_-4px_12px_rgba(0,0,0,0.06)]">
          <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3">
            <p className="text-sm text-slate-600">
              {seleccionadas.length} de {MAX_COMPARACION} propiedades seleccionadas para comparar
            </p>
            <div className="flex items-center gap-3">
              <button type="button" onClick={() => setSeleccionadas([])} className="text-sm text-slate-500 hover:text-slate-900">
                Limpiar
              </button>
              {seleccionadas.length >= 2 ? (
                <a
                  href={`/comparar?ids=${seleccionadas.join(',')}`}
                  className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
                >
                  Comparar
                </a>
              ) : (
                // <a> sin href no tiene rol "link" (ni foco de teclado) — un
                // <button disabled> es la forma correcta de expresar
                // "esta acción existe pero no está disponible todavía".
                <button
                  type="button"
                  disabled
                  className="cursor-not-allowed rounded-md bg-slate-300 px-4 py-2 text-sm font-semibold text-white"
                >
                  Comparar
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
