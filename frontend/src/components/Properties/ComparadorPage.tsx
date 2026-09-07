import type { ReactNode } from 'react';
import { useComparador } from '../../hooks/useComparador';
import { AlertTriangleIcon, CheckCircleIcon, CompareArrowsIcon, LocationPinIcon, RulerIcon } from '../common/icons';
import { TIPO_INMUEBLE_LABEL } from './propertyLabels';
import type { PropertyDetailResponse } from '../../types/properties';

function leerIdsDeLaUrl(): string[] {
  const params = new URLSearchParams(window.location.search);
  return (params.get('ids') ?? '').split(',').filter(Boolean);
}

export function ComparadorPage() {
  const ids = leerIdsDeLaUrl();
  // Menos de 2 ids no es un estado válido para comparar — ni vale la pena
  // pedirle nada al backend (ver el mensaje que se muestra en ese caso).
  const { propiedades, isLoading, error } = useComparador(ids.length >= 2 ? ids : []);

  return (
    <div className="min-h-screen bg-white">
      {/* Mismo header del Home y del detalle de propiedad (fases 7 y 8/N) —
          sticky con blur, ancho máx. 1440px de la grilla del showcase. */}
      <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/90 backdrop-blur supports-[backdrop-filter]:bg-white/75">
        <div className="mx-auto max-w-[1440px] px-6 py-4 sm:px-8">
          <a href="/" className="font-heading text-lg font-bold tracking-tight text-slate-900">
            Plataforma <span className="text-sky-600">Civil &amp; Inmobiliaria</span>
          </a>
        </div>
      </header>

      <main className="mx-auto max-w-[1440px] px-6 py-public-lg sm:px-8 sm:py-public-xl">
        <a
          href="/#propiedades"
          className="mb-5 inline-flex items-center gap-1.5 font-heading text-sm font-medium text-slate-500 transition hover:text-slate-900"
        >
          ← Volver al catálogo
        </a>

        <span className="font-heading text-xs font-bold uppercase tracking-widest text-sky-600">
          Herramienta de decisión inmobiliaria
        </span>
        <h1 className="mb-1 flex items-center gap-2 font-heading text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
          <CompareArrowsIcon className="h-7 w-7 text-sky-600" /> Cuadro comparativo técnico
        </h1>

        {ids.length < 2 ? (
          <div className="mt-8 rounded-2xl border border-slate-200 bg-white p-10 text-center shadow-[0_1px_3px_rgba(15,23,42,0.06)]">
            <p className="mx-auto max-w-md text-slate-600">
              Selecciona al menos 2 propiedades desde el catálogo (con el check "Comparar" en cada ficha) para verlas
              lado a lado aquí.
            </p>
            <a
              href="/#propiedades"
              className="font-heading mt-5 inline-block rounded-lg bg-slate-900 px-5 py-2.5 text-sm font-semibold text-white transition hover:-translate-y-0.5 hover:bg-slate-800"
            >
              Ir al catálogo
            </a>
          </div>
        ) : error ? (
          <p className="mt-6 text-red-600">{error}</p>
        ) : isLoading ? (
          <p className="mt-6 text-sm text-slate-500">Cargando…</p>
        ) : (
          <TablaComparativa propiedades={propiedades} />
        )}
      </main>
    </div>
  );
}

function TablaComparativa({ propiedades }: { propiedades: PropertyDetailResponse[] }) {
  return (
    <div className="mt-8 overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-[0_10px_25px_-5px_rgba(15,23,42,0.06),0_8px_10px_-6px_rgba(15,23,42,0.04)]">
      <table className="w-full min-w-[720px] table-fixed border-collapse text-left">
        <thead className="sticky top-0 z-10 bg-white shadow-[0_1px_0_rgba(15,23,42,0.08)]">
          <tr>
            <th className="w-48 p-4 align-top">
              <span className="font-heading text-sm font-bold text-slate-900">Parámetros técnicos</span>
              <span className="mt-1 block font-heading text-[11px] font-bold uppercase tracking-wide text-slate-500">
                {propiedades.length} activos seleccionados
              </span>
            </th>
            {propiedades.map((p) => (
              <th key={p.id} className="w-64 bg-white p-4 align-top">
                <a href={`/propiedades/${p.id}`} className="group flex flex-col gap-2 hover:opacity-90">
                  <div className="h-28 w-full overflow-hidden rounded-xl bg-slate-100">
                    {p.multimedia.find((m) => m.tipo === 'Foto') ? (
                      <img
                        src={p.multimedia.find((m) => m.tipo === 'Foto')!.url}
                        alt={p.titulo}
                        className="h-full w-full object-cover transition duration-300 group-hover:scale-105"
                      />
                    ) : (
                      <div className="flex h-full w-full items-center justify-center text-xs text-slate-400">Sin foto</div>
                    )}
                  </div>
                  <span className="font-heading text-sm font-semibold leading-snug text-slate-900">{p.titulo}</span>
                  <span className="font-heading text-base font-bold text-slate-900">
                    {p.precio.toLocaleString('es-CO')} {p.moneda}
                  </span>
                </a>
                <a
                  href={`/propiedades/${p.id}`}
                  className="mt-2 block rounded-lg bg-slate-900 px-3 py-2 text-center font-heading text-xs font-semibold text-white transition hover:bg-slate-800"
                >
                  Ver ficha completa
                </a>
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 text-sm text-slate-700">
          <Fila
            icono={<LocationPinIcon className="h-4 w-4 text-sky-600" />}
            label="Ubicación"
            propiedades={propiedades}
            render={(p) => `${p.municipio}, ${p.departamento}`}
          />
          <Fila label="Tipo" propiedades={propiedades} render={(p) => TIPO_INMUEBLE_LABEL[p.tipoInmueble] ?? p.tipoInmueble} />
          <Fila
            icono={<RulerIcon className="h-4 w-4 text-sky-600" />}
            label="Área terreno"
            propiedades={propiedades}
            render={(p) => (
              <span className="font-heading font-semibold">{p.areaTerrenoM2.toLocaleString('es-CO')} m²</span>
            )}
          />
          <Fila
            label="Área construida"
            propiedades={propiedades}
            render={(p) => (p.areaConstruidaM2 ? `${p.areaConstruidaM2.toLocaleString('es-CO')} m²` : '—')}
          />
          <Fila label="Pendiente del terreno" propiedades={propiedades} render={(p) => `${p.pendientePorcentaje}%`} />
          <Fila label="Topografía" propiedades={propiedades} render={(p) => p.topografia} />
          <Fila label="Tipo de suelo" propiedades={propiedades} render={(p) => p.tipoSuelo} />
          <Fila
            label="Viabilidad constructiva"
            propiedades={propiedades}
            render={(p) =>
              p.esViableConstructivamente ? (
                <span className="flex w-fit items-center gap-1 rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">
                  <CheckCircleIcon className="h-3.5 w-3.5" /> Viable
                </span>
              ) : (
                <span className="flex w-fit items-center gap-1 rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-800">
                  <AlertTriangleIcon className="h-3.5 w-3.5" /> Con restricciones
                </span>
              )
            }
          />
          <Fila
            label="Restricciones"
            propiedades={propiedades}
            render={(p) =>
              p.restriccionesViabilidad.length === 0 ? (
                '—'
              ) : (
                <ul className="list-disc space-y-1 pl-4 text-xs text-amber-800">
                  {p.restriccionesViabilidad.map((r, i) => (
                    <li key={i}>{r}</li>
                  ))}
                </ul>
              )
            }
          />
          <Fila
            label="Retiros ambientales"
            propiedades={propiedades}
            render={(p) =>
              p.retirosAmbientales.length === 0 ? (
                '—'
              ) : (
                <ul className="space-y-1 text-xs text-slate-600">
                  {p.retirosAmbientales.map((r, i) => (
                    <li key={i}>
                      {r.tipoFuente}: {r.distanciaMinimaMetros} m
                    </li>
                  ))}
                </ul>
              )
            }
          />
        </tbody>
      </table>
    </div>
  );
}

function Fila({
  icono,
  label,
  propiedades,
  render,
}: {
  icono?: ReactNode;
  label: string;
  propiedades: PropertyDetailResponse[];
  render: (p: PropertyDetailResponse) => ReactNode;
}) {
  return (
    <tr className="odd:bg-slate-50/60 hover:bg-slate-50">
      <td className="flex items-center gap-2 p-4 align-top font-heading text-sm font-semibold text-slate-900">
        {icono}
        {label}
      </td>
      {propiedades.map((p) => (
        <td key={p.id} className="p-4 align-top font-heading text-sm text-slate-700">
          {render(p)}
        </td>
      ))}
    </tr>
  );
}
