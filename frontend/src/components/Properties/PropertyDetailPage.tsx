import { useEffect } from 'react';
import { usePropertyDetail } from '../../hooks/usePropertyDetail';
import { PropertyInterestForm } from './PropertyInterestForm';
import { PhotoGallery } from './PhotoGallery';
import { PropertyLocationMap } from './PropertyLocationMap';
import { WhatsAppButton } from './WhatsAppButton';
import { SITE_TITLE } from '../../seo';

function usePageTitle(titulo: string | undefined) {
  // Google renderiza JS antes de indexar, así que esto sí ayuda al SEO de
  // cada ficha — a diferencia de Open Graph/Twitter Card (index.html,
  // estáticos), que los bots de redes sociales leen sin ejecutar JS y por
  // eso siempre muestran el título genérico del sitio al compartir un link.
  useEffect(() => {
    document.title = titulo ? `${titulo} | Plataforma Civil e Inmobiliaria` : SITE_TITLE;
    return () => {
      document.title = SITE_TITLE;
    };
  }, [titulo]);
}

export function PropertyDetailPage({ id }: { id: string }) {
  const { property, isLoading, error, notFound } = usePropertyDetail(id);
  usePageTitle(property?.titulo);

  if (isLoading) {
    return <div className="mx-auto max-w-4xl px-6 py-16 text-center text-slate-500">Cargando…</div>;
  }

  if (notFound) {
    return (
      <div className="mx-auto max-w-4xl px-6 py-16 text-center">
        <h1 className="text-xl font-bold text-slate-900">Propiedad no encontrada</h1>
        <a href="/" className="mt-4 inline-block text-emerald-700 hover:underline">
          Volver al catálogo
        </a>
      </div>
    );
  }

  if (error || !property) {
    return <div className="mx-auto max-w-4xl px-6 py-16 text-center text-red-600">{error ?? 'Error inesperado.'}</div>;
  }

  const fotos = property.multimedia.filter((m) => m.tipo === 'Foto');

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

        <PhotoGallery fotos={fotos} titulo={property.titulo} />

        <div className="grid grid-cols-1 gap-8 lg:grid-cols-3">
          <div className="lg:col-span-2">
            <div className="mb-4 flex items-start justify-between gap-4">
              <div>
                <h1 className="text-2xl font-bold text-slate-900">{property.titulo}</h1>
                <p className="text-slate-500">
                  {property.direccion}, {property.municipio}, {property.departamento}
                </p>
              </div>
              {property.esViableConstructivamente ? (
                <span className="whitespace-nowrap rounded-full bg-emerald-100 px-3 py-1 text-sm font-medium text-emerald-800">
                  Viable constructivamente
                </span>
              ) : (
                <span className="whitespace-nowrap rounded-full bg-amber-100 px-3 py-1 text-sm font-medium text-amber-800">
                  Con restricciones
                </span>
              )}
            </div>

            <p className="text-2xl font-bold text-slate-900">
              {property.precio.toLocaleString('es-CO')} {property.moneda}
            </p>

            <div className="my-6 grid grid-cols-2 gap-4 rounded-lg border border-slate-200 p-4 text-sm sm:grid-cols-4">
              <div>
                <p className="text-slate-500">Área terreno</p>
                <p className="font-semibold text-slate-900">{property.areaTerrenoM2.toLocaleString('es-CO')} m²</p>
              </div>
              {property.areaConstruidaM2 && (
                <div>
                  <p className="text-slate-500">Área construida</p>
                  <p className="font-semibold text-slate-900">{property.areaConstruidaM2.toLocaleString('es-CO')} m²</p>
                </div>
              )}
              <div>
                <p className="text-slate-500">Pendiente</p>
                <p className="font-semibold text-slate-900">{property.pendientePorcentaje}%</p>
              </div>
              <div>
                <p className="text-slate-500">Topografía</p>
                <p className="font-semibold text-slate-900">{property.topografia}</p>
              </div>
            </div>

            <h2 className="mb-2 font-semibold text-slate-900">Descripción</h2>
            <p className="mb-6 whitespace-pre-line text-slate-600">{property.descripcion}</p>

            <h2 className="mb-2 font-semibold text-slate-900">Viabilidad constructiva</h2>
            {property.restriccionesViabilidad.length === 0 ? (
              <p className="mb-6 text-sm text-emerald-700">
                Sin restricciones detectadas frente a las reglas de referencia (pendiente y retiros ambientales).
              </p>
            ) : (
              <ul className="mb-6 list-disc space-y-1 pl-5 text-sm text-amber-800">
                {property.restriccionesViabilidad.map((r, i) => (
                  <li key={i}>{r}</li>
                ))}
              </ul>
            )}

            <PropertyLocationMap latitud={property.latitud} longitud={property.longitud} titulo={property.titulo} />
          </div>

          <aside className="flex flex-col gap-4 rounded-xl border border-slate-200 bg-white p-5 shadow-sm lg:sticky lg:top-6 lg:h-fit">
            <WhatsAppButton mensaje={`Hola, me interesa la propiedad "${property.titulo}" (${property.municipio}, ${property.departamento}).`} />
            <PropertyInterestForm propiedadId={property.id} />
          </aside>
        </div>
      </main>
    </div>
  );
}
