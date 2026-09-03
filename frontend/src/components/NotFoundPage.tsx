import { useEffect } from 'react';
import { SITE_TITLE } from '../seo';

// gap #9 — antes cualquier ruta desconocida (typo, link roto, página vieja
// indexada) renderizaba en silencio el home completo: confuso para el
// visitante y para buscadores (nunca había una señal clara de "esto no
// existe"). Nota honesta: al ser un SPA sin servidor de renderizado, esta
// página nunca puede devolver un status HTTP 404 real — Azure Static Web
// Apps reescribe cualquier ruta desconocida a index.html con 200 (ver
// staticwebapp.config.json, navigationFallback). Lo máximo que se puede
// hacer del lado del cliente es esta señal visual + de título.
export function NotFoundPage() {
  useEffect(() => {
    document.title = `Página no encontrada | ${SITE_TITLE}`;
    return () => {
      document.title = SITE_TITLE;
    };
  }, []);

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-gradient-to-b from-slate-100 to-white px-6 text-center">
      <p className="text-sm font-semibold uppercase tracking-wide text-emerald-600">Error 404</p>
      <h1 className="mt-2 text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">Esta página no existe</h1>
      <p className="mx-auto mt-3 max-w-md text-slate-500">
        Puede que el enlace esté roto o que la página se haya movido. Revisa la dirección o vuelve al inicio.
      </p>

      <div className="mt-8 flex flex-col gap-3 sm:flex-row">
        <a
          href="/"
          className="rounded-lg bg-emerald-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-emerald-700"
        >
          Volver al inicio
        </a>
        <a
          href="/#propiedades"
          className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
        >
          Ver propiedades
        </a>
      </div>
    </div>
  );
}
