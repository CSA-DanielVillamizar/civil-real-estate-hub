import { useEffect, type ReactNode } from 'react';
import { AUTORIDAD_AMBIENTAL, getMunicipioBySlug } from '../../data/municipios';
import { SITE_TITLE } from '../../seo';

interface NormativaMunicipioPageProps {
  slug: string;
}

export function NormativaMunicipioPage({ slug }: NormativaMunicipioPageProps) {
  const municipio = getMunicipioBySlug(slug);

  useEffect(() => {
    document.title = municipio
      ? `Normativa de construcción en ${municipio.nombre}, Antioquia | Plataforma Civil e Inmobiliaria`
      : 'Municipio no encontrado | Plataforma Civil e Inmobiliaria';
    return () => {
      document.title = SITE_TITLE;
    };
  }, [municipio]);

  if (!municipio) {
    return (
      <div className="mx-auto max-w-2xl px-6 py-16 text-center">
        <h1 className="text-xl font-bold text-slate-900">Municipio no encontrado</h1>
        <a href="/normativa" className="mt-4 inline-block text-emerald-700 hover:underline">
          ← Ver todos los municipios
        </a>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-slate-100 to-white">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto max-w-6xl px-6 py-4">
          <a href="/" className="text-lg font-bold text-slate-900">
            Plataforma <span className="text-emerald-600">Civil &amp; Inmobiliaria</span>
          </a>
        </div>
      </header>

      <main className="mx-auto max-w-3xl px-6 py-10">
        <a href="/normativa" className="mb-4 inline-block text-sm text-slate-500 hover:text-slate-900">
          ← Todos los municipios
        </a>

        <h1 className="mb-1 text-2xl font-bold text-slate-900 sm:text-3xl">
          Normativa de construcción en {municipio.nombre}, Antioquia
        </h1>
        <p className="mb-8 text-slate-500">{municipio.resumen}</p>

        <div className="flex flex-col gap-6">
          <Bloque titulo="Autoridad ambiental">
            <p className="text-slate-600">
              {municipio.nombre} está en la jurisdicción de{' '}
              <a href={AUTORIDAD_AMBIENTAL.url} target="_blank" rel="noopener noreferrer" className="text-emerald-700 hover:underline">
                {AUTORIDAD_AMBIENTAL.nombreCompleto} ({AUTORIDAD_AMBIENTAL.nombre})
              </a>
              , la corporación autónoma regional que otorga permisos ambientales (vertimientos, aprovechamiento
              forestal, ocupación de cauce) en todo el Oriente Antioqueño.
            </p>
          </Bloque>

          <Bloque titulo="Instrumento de ordenamiento territorial">
            <p className="text-slate-600">{municipio.pot.instrumento}.</p>
            <p className="mt-2 text-slate-600">{municipio.pot.estado}</p>
            <FuenteLink url={municipio.pot.fuenteUrl} label={municipio.pot.fuenteLabel} />
          </Bloque>

          <Bloque titulo="¿Quién expide las licencias urbanísticas?">
            <p className="text-slate-600">{municipio.licencias.entidad}.</p>
            <FuenteLink url={municipio.licencias.fuenteUrl} label={municipio.licencias.fuenteLabel} />
          </Bloque>

          <Bloque titulo="Retiros ambientales y pendiente del terreno">
            <p className="text-slate-600">
              Nuestra calculadora de viabilidad usa como criterio interno de referencia un retiro mínimo de 15
              metros frente a fuentes hídricas y una pendiente máxima de 25% para construcción estándar — es un
              filtro conservador para una primera revisión, no la cifra legal definitiva. La norma nacional marco
              (Decreto 2811 de 1974 y Decreto 1076 de 2015) habla de una franja de aislamiento forestal protector de
              hasta 30 metros junto a cauces, y el retiro exacto aplicable a un predio específico lo determina{' '}
              {AUTORIDAD_AMBIENTAL.nombre} junto con el instrumento de ordenamiento vigente.
            </p>
          </Bloque>

          {municipio.notas && (
            <Bloque titulo="A tener en cuenta">
              <p className="text-slate-600">{municipio.notas}</p>
            </Bloque>
          )}

          <Bloque titulo="Otros aspectos que conviene verificar antes de comprar">
            <ul className="list-disc space-y-1 pl-5 text-sm text-slate-600">
              <li>Uso del suelo del predio específico (rural, suburbano o de protección) según el POT/PBOT vigente.</li>
              <li>Factibilidad de servicios públicos (acueducto veredal, energía) — común en lotes rurales.</li>
              <li>Si el predio está en zona de riesgo no mitigable (Ley 1523 de 2012).</li>
            </ul>
          </Bloque>
        </div>

        <div className="mt-10 rounded-xl border border-emerald-200 bg-emerald-50 p-6">
          <p className="text-sm text-emerald-900">
            Esta página es información general orientativa, no asesoría legal — no reemplaza una consulta de norma
            urbanística ni un estudio de viabilidad ambiental sobre el predio específico.
          </p>
          <a
            href="/#viabilidad-ambiental"
            className="mt-3 inline-block rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700"
          >
            Solicitar estudio de viabilidad ambiental
          </a>
        </div>
      </main>
    </div>
  );
}

function Bloque({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <section className="rounded-xl border border-slate-200 bg-white p-5">
      <h2 className="mb-2 font-semibold text-slate-900">{titulo}</h2>
      {children}
    </section>
  );
}

function FuenteLink({ url, label }: { url: string; label: string }) {
  return (
    <p className="mt-3 text-xs text-slate-400">
      Fuente:{' '}
      <a href={url} target="_blank" rel="noopener noreferrer" className="text-slate-500 underline hover:text-slate-700">
        {label}
      </a>
    </p>
  );
}
