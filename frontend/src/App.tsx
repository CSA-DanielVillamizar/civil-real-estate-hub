import { BudgetCalculator } from './components/BudgetCalculator/BudgetCalculator';
import { ViabilidadAmbientalAdminPage } from './components/Admin/ViabilidadAmbientalAdminPage';
import { PropertiesAdminPage } from './components/Admin/PropertiesAdminPage';
import { LeadsAdminPage } from './components/Admin/LeadsAdminPage';
import { ViabilidadAmbientalSection } from './components/ViabilidadAmbiental/ViabilidadAmbientalSection';
import { PropertiesSection } from './components/Properties/PropertiesSection';
import { PropertyDetailPage } from './components/Properties/PropertyDetailPage';
import { ConsultoriaEstructuralSection } from './components/Servicios/ConsultoriaEstructuralSection';
import { InterventoriaSection } from './components/Servicios/InterventoriaSection';

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

  const detalleMatch = path.match(/^\/propiedades\/([0-9a-fA-F-]{36})$/);
  if (detalleMatch) {
    return <PropertyDetailPage id={detalleMatch[1]} />;
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-slate-100 to-white">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <span className="text-lg font-bold text-slate-900">
            Plataforma <span className="text-emerald-600">Civil &amp; Inmobiliaria</span>
          </span>
          <nav className="hidden gap-6 text-sm font-medium text-slate-600 sm:flex">
            <a href="#calculadora" className="hover:text-slate-900">
              Calculadora
            </a>
            <a href="#viabilidad-ambiental" className="hover:text-slate-900">
              Viabilidad ambiental
            </a>
            <a href="#propiedades" className="hover:text-slate-900">
              Propiedades
            </a>
            <a href="#consultoria-estructural" className="hover:text-slate-900">
              Consultoría estructural
            </a>
            <a href="#interventoria" className="hover:text-slate-900">
              Interventoría
            </a>
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-12">
        <div className="mb-10 text-center">
          <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
            Ingeniería y bienes raíces, en un solo lugar
          </h1>
          <p className="mx-auto mt-3 max-w-2xl text-slate-500">
            Consulta propiedades, presupuesta tu obra y conecta con nuestro equipo de consultoría e interventoría.
          </p>
        </div>

        <div id="calculadora">
          <BudgetCalculator />
        </div>

        <div id="viabilidad-ambiental" className="mt-12">
          <ViabilidadAmbientalSection />
        </div>

        <div id="propiedades" className="mt-12">
          <PropertiesSection />
        </div>

        <div id="consultoria-estructural" className="mt-12">
          <ConsultoriaEstructuralSection />
        </div>

        <div id="interventoria" className="mt-12">
          <InterventoriaSection />
        </div>
      </main>
    </div>
  );
}

export default App;
