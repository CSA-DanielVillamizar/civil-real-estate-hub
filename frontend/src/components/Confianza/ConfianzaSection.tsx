import { useContenidoConfianzaPublicado } from '../../hooks/useContenidoConfianzaPublicado';
import { CheckCircleIcon, LocationPinIcon } from '../common/icons';
import { TipoContenidoConfianza, type ContenidoConfianza } from '../../types/confianza';

// Testimonios de clientes + casos de portafolio — sobre todo para
// consultoría estructural e interventoría, que hoy no tienen ninguna prueba
// social en el sitio (gap #4). Si no hay nada publicado todavía, la sección
// completa no se renderiza (no tiene sentido mostrar un bloque vacío).
export function ConfianzaSection() {
  const { items, isLoading, error } = useContenidoConfianzaPublicado();

  if (isLoading || error || items.length === 0) return null;

  const testimonios = items.filter((item) => item.tipo === TipoContenidoConfianza.Testimonio);
  const portafolio = items.filter((item) => item.tipo === TipoContenidoConfianza.Portafolio);

  return (
    <section className="flex flex-col gap-public-xl">
      {testimonios.length > 0 && (
        <div>
          <div className="mb-8 text-center">
            <p className="font-heading text-xs font-bold uppercase tracking-widest text-sky-600">Confianza</p>
            <h2 className="font-heading mt-1 text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
              Lo que dicen nuestros clientes
            </h2>
          </div>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {testimonios.map((item) => (
              <TestimonioCard key={item.id} item={item} />
            ))}
          </div>
        </div>
      )}

      {portafolio.length > 0 && (
        <div>
          <div className="mb-8 text-center">
            <p className="font-heading text-xs font-bold uppercase tracking-widest text-sky-600">Portafolio</p>
            <h2 className="font-heading mt-1 text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
              Proyectos entregados
            </h2>
          </div>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {portafolio.map((item) => (
              <PortafolioCard key={item.id} item={item} />
            ))}
          </div>
        </div>
      )}
    </section>
  );
}

// Iniciales del nombre del cliente para un avatar circular liviano — nunca
// una foto inventada; solo texto real ya presente en el testimonio.
function iniciales(nombre: string): string {
  const partes = nombre.trim().split(/\s+/);
  return ((partes[0]?.[0] ?? '') + (partes[1]?.[0] ?? '')).toUpperCase();
}

function TestimonioCard({ item }: { item: ContenidoConfianza }) {
  return (
    <figure className="flex flex-col rounded-2xl border border-slate-200 bg-white p-6 shadow-[0_1px_3px_rgba(15,23,42,0.06)] transition duration-300 hover:-translate-y-1 hover:shadow-[0_10px_25px_-5px_rgba(15,23,42,0.08)]">
      <span aria-hidden="true" className="font-heading text-4xl font-bold leading-none text-slate-200">
        &ldquo;
      </span>
      <blockquote className="-mt-2 flex-1 text-slate-700">{item.descripcion}</blockquote>
      <figcaption className="mt-5 flex items-center gap-3 border-t border-slate-100 pt-4">
        <span className="font-heading flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-sky-100 text-xs font-bold text-sky-700">
          {iniciales(item.titulo)}
        </span>
        <span className="font-heading text-sm font-semibold text-slate-900">
          {item.titulo}
          {item.municipio && <span className="block font-sans text-xs font-normal text-slate-500">{item.municipio}</span>}
        </span>
      </figcaption>
    </figure>
  );
}

function PortafolioCard({ item }: { item: ContenidoConfianza }) {
  return (
    <article className="flex flex-col rounded-2xl border border-slate-200 bg-white p-6 shadow-[0_1px_3px_rgba(15,23,42,0.06)] transition duration-300 hover:-translate-y-1 hover:shadow-[0_10px_25px_-5px_rgba(15,23,42,0.08)]">
      <span className="flex w-fit items-center gap-1 rounded-full bg-emerald-100 px-2 py-1 font-heading text-[11px] font-bold uppercase tracking-wide text-emerald-800">
        <CheckCircleIcon className="h-3 w-3" /> Entregado
      </span>
      <h3 className="font-heading mt-3 font-semibold text-slate-900">{item.titulo}</h3>
      {item.municipio && (
        <p className="mt-1 flex items-center gap-1 text-xs text-slate-500">
          <LocationPinIcon className="h-3.5 w-3.5 text-sky-600" /> {item.municipio}
        </p>
      )}
      <p className="mt-2 flex-1 text-sm text-slate-600">{item.descripcion}</p>
    </article>
  );
}
