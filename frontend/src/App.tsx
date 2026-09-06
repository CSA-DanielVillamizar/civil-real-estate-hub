import { BudgetCalculator } from './components/BudgetCalculator/BudgetCalculator';
import { ViabilidadAmbientalAdminPage } from './components/Admin/ViabilidadAmbientalAdminPage';
import { PropertiesAdminPage } from './components/Admin/PropertiesAdminPage';
import { LeadsAdminPage } from './components/Admin/LeadsAdminPage';
import { ObrasAdminPage } from './components/Admin/ObrasAdminPage';
import { ProyectoObraAdminPage } from './components/Admin/ProyectoObraAdminPage';
import { MiObraPage } from './components/Obras/MiObraPage';
import { UsuariosAdminPage } from './components/Admin/UsuariosAdminPage';
import { ConfianzaAdminPage } from './components/Admin/ConfianzaAdminPage';
import { ConfianzaSection } from './components/Confianza/ConfianzaSection';
import { TarifasAdminPage } from './components/Admin/TarifasAdminPage';
import { ViabilidadAmbientalSection } from './components/ViabilidadAmbiental/ViabilidadAmbientalSection';
import { PropertiesSection } from './components/Properties/PropertiesSection';
import { PropertyDetailPage } from './components/Properties/PropertyDetailPage';
import { ComparadorPage } from './components/Properties/ComparadorPage';
import { NormativaIndexPage } from './components/Normativa/NormativaIndexPage';
import { NormativaMunicipioPage } from './components/Normativa/NormativaMunicipioPage';
import { NormativaTeaserSection } from './components/Normativa/NormativaTeaserSection';
import { ConsultoriaEstructuralSection } from './components/Servicios/ConsultoriaEstructuralSection';
import { InterventoriaSection } from './components/Servicios/InterventoriaSection';
import { PoliticaPrivacidadPage } from './components/Legal/PoliticaPrivacidadPage';
import { NotFoundPage } from './components/NotFoundPage';
import { WhatsAppIcon } from './components/common/icons';
import { WHATSAPP_NUMBER } from './config';

function App() {
  // Ruteo mínimo por path — no se agrega react-router para un puñado de
  // rutas fijas, coherente con la filosofía de dependencias mínimas del
  // proyecto (ver docs/02-business-case.md §6, FinOps).
  const path = window.location.pathname;

  if (path === '/admin/viabilidad-ambiental') {
    return <ViabilidadAmbientalAdminPage />;
  }

  if (path === '/admin/propiedades') {
    return <PropertiesAdminPage />;
  }

  if (path === '/admin/leads') {
    return <LeadsAdminPage />;
  }

  if (path === '/admin/obras') {
    return <ObrasAdminPage />;
  }

  if (path === '/admin/usuarios') {
    return <UsuariosAdminPage />;
  }

  if (path === '/admin/confianza') {
    return <ConfianzaAdminPage />;
  }

  if (path === '/admin/tarifas') {
    return <TarifasAdminPage />;
  }

  const obraAdminMatch = path.match(/^\/admin\/obras\/([0-9a-fA-F-]{36})$/);
  if (obraAdminMatch) {
    return <ProyectoObraAdminPage id={obraAdminMatch[1]} />;
  }

  const miObraMatch = path.match(/^\/mi-obra\/([\w-]+)$/);
  if (miObraMatch) {
    return <MiObraPage token={miObraMatch[1]} />;
  }

  const detalleMatch = path.match(/^\/propiedades\/([0-9a-fA-F-]{36})$/);
  if (detalleMatch) {
    return <PropertyDetailPage id={detalleMatch[1]} />;
  }

  if (path === '/comparar') {
    return <ComparadorPage />;
  }

  if (path === '/normativa') {
    return <NormativaIndexPage />;
  }

  const normativaMatch = path.match(/^\/normativa\/([a-z-]+)$/);
  if (normativaMatch) {
    return <NormativaMunicipioPage slug={normativaMatch[1]} />;
  }

  if (path === '/politica-de-privacidad') {
    return <PoliticaPrivacidadPage />;
  }

  // Cualquier ruta que no haya calzado con nada de arriba (gap #9): antes
  // caía en silencio al home completo, como si la URL rota fuera válida.
  if (path !== '/') {
    return <NotFoundPage />;
  }

  return (
    <div className="min-h-screen bg-white">
      {/* Accesibilidad (gap #8): invisible salvo con foco de teclado — deja
          saltar directo al contenido sin tabular por los 6 links del nav
          en cada carga de página. Primer elemento enfocable del documento. */}
      <a
        href="#contenido-principal"
        className="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-50 focus:rounded-md focus:bg-slate-900 focus:px-4 focus:py-2 focus:text-sm focus:font-semibold focus:text-white"
      >
        Saltar al contenido principal
      </a>

      {/* Grilla fluida de 12 columnas del showcase público (DESIGN.md
          §Layout & Spacing), centrada a 1440px máx — más ancha que el
          max-w-6xl (1152px) que sigue usando el Panel de Admin de alta
          densidad; son dos modos de layout deliberadamente distintos. */}
      <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/90 backdrop-blur supports-[backdrop-filter]:bg-white/75">
        <div className="mx-auto flex max-w-[1440px] items-center justify-between px-6 py-4 sm:px-8">
          <span className="font-heading text-lg font-bold tracking-tight text-slate-900">
            Plataforma <span className="text-sky-600">Civil &amp; Inmobiliaria</span>
          </span>
          <nav aria-label="Principal" className="hidden gap-6 font-heading text-sm font-medium text-slate-600 sm:flex">
            <a href="#calculadora" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Calculadora
            </a>
            <a href="#viabilidad-ambiental" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Viabilidad ambiental
            </a>
            <a href="#propiedades" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Propiedades
            </a>
            <a href="#consultoria-estructural" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Consultoría estructural
            </a>
            <a href="#interventoria" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Interventoría
            </a>
            <a href="/normativa" className="border-b-2 border-transparent pb-1 transition hover:border-sky-500 hover:text-slate-900">
              Normativa
            </a>
          </nav>
        </div>
      </header>

      {/* Hero: tipografía "headline-hero" de DESIGN.md (Inter, tracking
          apretado) y espaciado vertical generoso (public-spacious-xl/2xl)
          para la experiencia editorial del showcase — en contraste con la
          densidad del Panel de Admin. */}
      <div className="border-b border-slate-100 bg-gradient-to-b from-slate-50 to-white">
        <div className="mx-auto max-w-[1440px] px-6 py-public-xl text-center sm:px-8 sm:py-public-2xl">
          <h1 className="font-heading mx-auto max-w-3xl text-4xl font-bold tracking-tight text-slate-900 sm:text-5xl lg:text-6xl lg:leading-[1.05]">
            Ingeniería y bienes raíces, en un solo lugar
          </h1>
          <p className="mx-auto mt-5 max-w-2xl text-base text-slate-600 sm:text-lg">
            Consulta propiedades, presupuesta tu obra y conecta con nuestro equipo de consultoría e interventoría.
          </p>
          <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
            <a
              href="#propiedades"
              className="font-heading rounded-lg bg-slate-900 px-6 py-3 text-sm font-semibold text-white shadow-[0_10px_25px_-5px_rgba(15,23,42,0.15)] transition hover:-translate-y-0.5 hover:bg-slate-800"
            >
              Ver propiedades
            </a>
            <a
              href="#calculadora"
              className="font-heading rounded-lg border border-slate-300 bg-white px-6 py-3 text-sm font-semibold text-slate-700 transition hover:-translate-y-0.5 hover:bg-slate-50"
            >
              Cotiza tu obra
            </a>
          </div>
        </div>
      </div>

      <main id="contenido-principal" className="mx-auto max-w-[1440px] px-6 py-public-xl sm:px-8">
        <div id="calculadora">
          <BudgetCalculator />
        </div>

        <div id="viabilidad-ambiental" className="mt-public-xl">
          <ViabilidadAmbientalSection />
        </div>

        <div id="propiedades" className="mt-public-xl">
          <PropertiesSection />
        </div>

        <div id="consultoria-estructural" className="mt-public-xl">
          <ConsultoriaEstructuralSection />
        </div>

        <div id="interventoria" className="mt-public-xl">
          <InterventoriaSection />
        </div>

        <div className="mt-public-xl">
          <ConfianzaSection />
        </div>

        <div className="mt-public-xl">
          <NormativaTeaserSection />
        </div>
      </main>

      <footer className="border-t border-slate-200 bg-white py-public-lg">
        <div className="mx-auto flex max-w-[1440px] flex-col items-center justify-between gap-2 px-6 text-sm text-slate-500 sm:flex-row sm:px-8">
          <span className="font-heading font-semibold text-slate-700">
            © {new Date().getFullYear()} Plataforma Civil &amp; Inmobiliaria
          </span>
          <a href="/politica-de-privacidad" className="hover:text-slate-900 hover:underline">
            Política de privacidad
          </a>
        </div>
      </footer>

      {/* Canal de conversión de máxima visibilidad (WhatsApp), fijo sobre
          todo el flujo público — número real de WhatsApp Business (ver
          config.ts); se oculta solo si esa constante quedara vacía, mismo
          criterio que WhatsAppButton. */}
      {WHATSAPP_NUMBER && (
        <a
          href={`https://wa.me/${WHATSAPP_NUMBER}?text=${encodeURIComponent('Hola, quiero más información sobre sus servicios.')}`}
          target="_blank"
          rel="noopener noreferrer"
          aria-label="Escríbenos por WhatsApp"
          className="fixed bottom-6 right-6 z-40 flex h-14 w-14 items-center justify-center rounded-full bg-[#25D366] text-white shadow-[0_10px_25px_-5px_rgba(15,23,42,0.35)] transition hover:scale-105 hover:bg-[#1ebe57]"
        >
          <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-[#25D366] opacity-40 motion-reduce:animate-none" />
          <WhatsAppIcon className="relative h-7 w-7" />
        </a>
      )}
    </div>
  );
}

export default App;
