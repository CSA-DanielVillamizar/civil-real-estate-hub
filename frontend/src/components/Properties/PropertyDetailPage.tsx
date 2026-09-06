import { useEffect } from 'react';
import { usePropertyDetail } from '../../hooks/usePropertyDetail';
import { PropertyInterestForm } from './PropertyInterestForm';
import { PhotoGallery } from './PhotoGallery';
import { PropertyLocationMap } from './PropertyLocationMap';
import { WhatsAppButton } from './WhatsAppButton';
import { AlertTriangleIcon, CheckCircleIcon, LocationPinIcon } from '../common/icons';
import { TIPO_INMUEBLE_LABEL } from './propertyLabels';
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
        <h1 className="font-heading text-xl font-bold text-slate-900">Propiedad no encontrada</h1>
        <a href="/" className="mt-4 inline-block text-sky-700 hover:underline">
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
          <a href="/" className="font-heading text-lg font-bold text-slate-900">
            Plataforma <span className="text-sky-600">Civil &amp; Inmobiliaria</span>
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
            <div className="mb-3 flex flex-wrap items-center gap-2">
              <span className="rounded bg-slate-900 px-2 py-1 font-heading text-[11px] font-bold uppercase tracking-wider text-white">
                {TIPO_INMUEBLE_LABEL[property.tipoInmueble] ?? property.tipoInmueble}
              </span>
              {property.esViableConstructivamente ? (
                <span className="flex items-center gap-1 whitespace-nowrap rounded-full bg-emerald-100 px-3 py-1 text-sm font-medium text-emerald-800">
                  <CheckCircleIcon className="h-4 w-4" /> Viable constructivamente
                </span>
              ) : (
                <span className="flex items-center gap-1 whitespace-nowrap rounded-full bg-amber-100 px-3 py-1 text-sm font-medium text-amber-800">
                  <AlertTriangleIcon className="h-4 w-4" /> Con restricciones
                </span>
              )}
            </div>

            <h1 className="font-heading text-2xl font-bold tracking-tight text-slate-900">{property.titulo}</h1>
            <p className="mt-1 flex items-center gap-1 text-slate-500">
              <LocationPinIcon className="h-4 w-4 text-sky-600" />
              {property.direccion}, {property.municipio}, {property.departamento}
            </p>

            <p className="mt-4 font-heading text-3xl font-bold tracking-tight text-slate-900">
              {property.precio.toLocaleString('es-CO')}{' '}
              <span className="text-lg font-semibold text-slate-500">{property.moneda}</span>
            </p>

            {/* Ficha técnica — solo campos reales del dominio (área, pendiente,
                tipo de suelo, topografía), en el mismo formato de tarjeta
                escaneable de datos técnicos del nuevo sistema de diseño. */}
            <div className="my-6 grid grid-cols-2 gap-3 rounded-xl border border-slate-200 bg-slate-50 p-4 sm:grid-cols-4">
              <div className="flex flex-col">
                <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Área terreno</span>
                <span className="font-heading text-sm font-semibold text-slate-900">
                  {property.areaTerrenoM2.toLocaleString('es-CO')} m²
                </span>
              </div>
              {property.areaConstruidaM2 && (
                <div className="flex flex-col">
                  <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Área construida</span>
                  <span className="font-heading text-sm font-semibold text-slate-900">
                    {property.areaConstruidaM2.toLocaleString('es-CO')} m²
                  </span>
                </div>
              )}
              <div className="flex flex-col">
                <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Pendiente</span>
                <span className="font-heading text-sm font-semibold text-slate-900">{property.pendientePorcentaje}%</span>
              </div>
              <div className="flex flex-col">
                <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Topografía</span>
                <span className="font-heading text-sm font-semibold text-slate-900">{property.topografia}</span>
              </div>
              <div className="flex flex-col">
                <span className="font-heading text-[11px] font-medium uppercase tracking-wide text-slate-500">Tipo de suelo</span>
                <span className="font-heading text-sm font-semibold text-slate-900">{property.tipoSuelo}</span>
              </div>
            </div>

            <h2 className="mb-2 font-heading font-semibold text-slate-900">Descripción</h2>
            <p className="mb-6 whitespace-pre-line text-slate-600">{property.descripcion}</p>

            <h2 className="mb-2 font-heading font-semibold text-slate-900">Viabilidad constructiva</h2>
            {property.restriccionesViabilidad.length === 0 ? (
              <p className="mb-6 flex items-center gap-2 rounded-lg bg-emerald-50 p-3 text-sm text-emerald-800">
                <CheckCircleIcon className="h-4 w-4 shrink-0" />
                Sin restricciones detectadas frente a las reglas de referencia (pendiente y retiros ambientales).
              </p>
            ) : (
              <ul className="mb-6 flex flex-col gap-2">
                {property.restriccionesViabilidad.map((r, i) => (
                  <li key={i} className="flex items-start gap-2 rounded-lg bg-amber-50 p-3 text-sm text-amber-800">
                    <AlertTriangleIcon className="mt-0.5 h-4 w-4 shrink-0" />
                    <span>{r}</span>
                  </li>
                ))}
              </ul>
            )}

            {property.retirosAmbientales.length > 0 && (
              <>
                <h2 className="mb-2 font-heading font-semibold text-slate-900">Retiros ambientales</h2>
                <ul className="mb-6 flex flex-wrap gap-2">
                  {property.retirosAmbientales.map((r, i) => (
                    <li
                      key={i}
                      className="rounded-full bg-sky-50 px-3 py-1 font-heading text-xs font-medium text-sky-800"
                    >
                      {r.tipoFuente}: {r.distanciaMinimaMetros} m
                    </li>
                  ))}
                </ul>
              </>
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
