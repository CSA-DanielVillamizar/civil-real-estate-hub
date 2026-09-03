import { useContenidoConfianzaPublicado } from '../../hooks/useContenidoConfianzaPublicado';
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
    <section className="flex flex-col gap-12">
      {testimonios.length > 0 && (
        <div>
          <div className="mb-6 text-center">
            <h2 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">Lo que dicen nuestros clientes</h2>
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
          <div className="mb-6 text-center">
            <h2 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">Proyectos entregados</h2>
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

function TestimonioCard({ item }: { item: ContenidoConfianza }) {
  return (
    <figure className="flex flex-col rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <blockquote className="flex-1 text-slate-700">"{item.descripcion}"</blockquote>
      <figcaption className="mt-4 text-sm font-semibold text-slate-900">
        {item.titulo}
        {item.municipio && <span className="font-normal text-slate-500"> · {item.municipio}</span>}
      </figcaption>
    </figure>
  );
}

function PortafolioCard({ item }: { item: ContenidoConfianza }) {
  return (
    <article className="flex flex-col rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <h3 className="font-semibold text-slate-900">{item.titulo}</h3>
      {item.municipio && <p className="text-xs text-slate-500">{item.municipio}</p>}
      <p className="mt-2 flex-1 text-sm text-slate-600">{item.descripcion}</p>
    </article>
  );
}
