import type { ReactNode } from 'react';
import { useComparador } from '../../hooks/useComparador';
import type { PropertyDetailResponse } from '../../types/properties';

const TIPO_INMUEBLE_LABEL: Record<string, string> = {
  Lote: 'Lote',
  Casa: 'Casa',
  Apartamento: 'Apartamento',
  Local: 'Local comercial',
  Bodega: 'Bodega',
  Finca: 'Finca',
};

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
    <div className="min-h-screen bg-gradient-to-b from-slate-100 to-white">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto max-w-6xl px-6 py-4">
          <a href="/" className="text-lg font-bold text-slate-900">
            Plataforma <span className="text-emerald-600">Civil &amp; Inmobiliaria</span>
          </a>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-10">
        <a href="/#propiedades" className="mb-4 inline-block text-sm text-slate-500 hover:text-slate-900">
          ← Volver al catálogo
        </a>

        <h1 className="mb-1 text-2xl font-bold text-slate-900">Comparar propiedades</h1>

        {ids.length < 2 ? (
          <div className="mt-6 rounded-lg border border-slate-200 bg-white p-8 text-center">
            <p className="text-slate-600">
              Selecciona al menos 2 propiedades desde el catálogo (con el check "Comparar" en cada ficha) para verlas
              lado a lado aquí.
            </p>
            <a href="/#propiedades" className="mt-4 inline-block text-emerald-700 hover:underline">
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
    <div className="mt-6 overflow-x-auto rounded-lg border border-slate-200 bg-white">
      <table className="w-full min-w-[640px] table-fixed border-collapse text-sm">
        <thead>
          <tr className="border-b border-slate-200">
            <th className="w-40 p-4 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Propiedad
            </th>
            {propiedades.map((p) => (
              <th key={p.id} className="p-4 text-left align-top">
                <a href={`/propiedades/${p.id}`} className="block hover:opacity-80">
                  {p.multimedia.find((m) => m.tipo === 'Foto') ? (
                    <img
                      src={p.multimedia.find((m) => m.tipo === 'Foto')!.url}
                      alt={p.titulo}
                      className="mb-2 h-32 w-full rounded-md object-cover"
                    />
                  ) : (
                    <div className="mb-2 flex h-32 w-full items-center justify-center rounded-md bg-slate-100 text-xs text-slate-400">
                      Sin foto
                    </div>
                  )}
                  <span className="font-semibold text-slate-900">{p.titulo}</span>
                </a>
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          <Fila
            label="Precio"
            propiedades={propiedades}
            render={(p) => (
              <span className="font-semibold text-slate-900">
                {p.precio.toLocaleString('es-CO')} {p.moneda}
              </span>
            )}
          />
          <Fila label="Tipo" propiedades={propiedades} render={(p) => TIPO_INMUEBLE_LABEL[p.tipoInmueble] ?? p.tipoInmueble} />
          <Fila label="Ubicación" propiedades={propiedades} render={(p) => `${p.municipio}, ${p.departamento}`} />
          <Fila
            label="Área terreno"
            propiedades={propiedades}
            render={(p) => `${p.areaTerrenoM2.toLocaleString('es-CO')} m²`}
          />
          <Fila
            label="Área construida"
            propiedades={propiedades}
            render={(p) => (p.areaConstruidaM2 ? `${p.areaConstruidaM2.toLocaleString('es-CO')} m²` : '—')}
          />
          <Fila label="Pendiente" propiedades={propiedades} render={(p) => `${p.pendientePorcentaje}%`} />
          <Fila label="Topografía" propiedades={propiedades} render={(p) => p.topografia} />
          <Fila
            label="Viabilidad constructiva"
            propiedades={propiedades}
            render={(p) =>
              p.esViableConstructivamente ? (
                <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800">Viable</span>
              ) : (
                <span className="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-800">Con restricciones</span>
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
  label,
  propiedades,
  render,
}: {
  label: string;
  propiedades: PropertyDetailResponse[];
  render: (p: PropertyDetailResponse) => ReactNode;
}) {
  return (
    <tr>
      <td className="p-4 align-top text-xs font-semibold uppercase tracking-wide text-slate-500">{label}</td>
      {propiedades.map((p) => (
        <td key={p.id} className="p-4 align-top text-slate-700">
          {render(p)}
        </td>
      ))}
    </tr>
  );
}
