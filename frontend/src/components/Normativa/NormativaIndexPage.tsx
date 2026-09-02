import { useEffect } from 'react';
import { MUNICIPIOS } from '../../data/municipios';
import { SITE_TITLE } from '../../seo';

export function NormativaIndexPage() {
  useEffect(() => {
    document.title = 'Normativa de construcción por municipio (Oriente Antioqueño) | Plataforma Civil e Inmobiliaria';
    return () => {
      document.title = SITE_TITLE;
    };
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-b from-slate-100 to-white">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto max-w-6xl px-6 py-4">
          <a href="/" className="text-lg font-bold text-slate-900">
            Plataforma <span className="text-emerald-600">Civil &amp; Inmobiliaria</span>
          </a>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-6 py-10">
        <a href="/" className="mb-4 inline-block text-sm text-slate-500 hover:text-slate-900">
          ← Volver al inicio
        </a>

        <h1 className="mb-2 text-2xl font-bold text-slate-900 sm:text-3xl">
          Normativa de construcción por municipio
        </h1>
        <p className="max-w-2xl text-slate-500">
          Antes de comprar un lote o proyectar una construcción en el Oriente Antioqueño, conviene saber qué
          instrumento de ordenamiento territorial rige el municipio y quién expide las licencias urbanísticas. Esto
          es información general orientativa, no asesoría legal — la norma aplicable a un predio específico solo la
          determina la autoridad competente.
        </p>

        <div className="mt-8 grid grid-cols-1 gap-4 sm:grid-cols-2">
          {MUNICIPIOS.map((m) => (
            <a
              key={m.slug}
              href={`/normativa/${m.slug}`}
              className="group flex flex-col gap-1.5 rounded-xl border border-slate-200 bg-white p-5 shadow-sm transition hover:shadow-md"
            >
              <h2 className="font-semibold text-slate-900 group-hover:text-emerald-700">{m.nombre}</h2>
              <p className="text-sm text-slate-500">{m.resumen}</p>
            </a>
          ))}
        </div>
      </main>
    </div>
  );
}
