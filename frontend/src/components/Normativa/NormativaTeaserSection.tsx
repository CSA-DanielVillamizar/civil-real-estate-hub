import { MUNICIPIOS } from '../../data/municipios';

export function NormativaTeaserSection() {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-6 text-center sm:p-8">
      <h2 className="text-2xl font-bold text-slate-900">Normativa de construcción por municipio</h2>
      <p className="mx-auto mt-2 max-w-2xl text-slate-500">
        Qué instrumento de ordenamiento rige y quién expide las licencias urbanísticas en cada municipio del
        Oriente Antioqueño donde operamos.
      </p>

      <div className="mt-5 flex flex-wrap justify-center gap-2">
        {MUNICIPIOS.map((m) => (
          <a
            key={m.slug}
            href={`/normativa/${m.slug}`}
            className="rounded-full border border-slate-200 px-4 py-1.5 text-sm font-medium text-slate-700 hover:border-emerald-300 hover:text-emerald-700"
          >
            {m.nombre}
          </a>
        ))}
      </div>
    </section>
  );
}
