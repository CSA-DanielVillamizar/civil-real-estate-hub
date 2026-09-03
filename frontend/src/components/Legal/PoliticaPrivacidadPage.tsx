import { useEffect, type ReactNode } from 'react';
import { SITE_TITLE } from '../../seo';
import { WHATSAPP_NUMBER } from '../../config';

export function PoliticaPrivacidadPage() {
  useEffect(() => {
    document.title = 'Política de privacidad | Plataforma Civil e Inmobiliaria';
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

      <main className="mx-auto max-w-3xl px-6 py-10">
        <a href="/" className="mb-4 inline-block text-sm text-slate-500 hover:text-slate-900">
          ← Volver al inicio
        </a>

        <h1 className="mb-2 text-2xl font-bold text-slate-900 sm:text-3xl">Política de tratamiento de datos personales</h1>
        <p className="mb-8 text-sm text-slate-500">
          Elaborada conforme a la Ley 1581 de 2012 y el Decreto 1377 de 2013 de Colombia (protección de datos
          personales — "Habeas Data").
        </p>

        <div className="flex flex-col gap-6">
          <Bloque titulo="¿Qué datos recogemos?">
            <p>
              Cuando usas la calculadora de obra, solicitas un estudio de viabilidad ambiental, muestras interés en
              una propiedad, o pides información sobre consultoría estructural o interventoría, recogemos: tu
              nombre, correo electrónico y número de teléfono. En algunos formularios también pedimos el municipio
              o departamento del predio, y un mensaje libre si decides contarnos más sobre tu proyecto.
            </p>
          </Bloque>

          <Bloque titulo="¿Para qué usamos tus datos?">
            <ul className="list-disc space-y-1 pl-5">
              <li>Contactarte para dar seguimiento a tu solicitud (cotización, estudio, consultoría, interventoría).</li>
              <li>Enviarte la estimación o el resultado que solicitaste (ej. el presupuesto de obra en PDF).</li>
              <li>Mejorar nuestros servicios y entender qué necesita nuestra base de clientes.</li>
            </ul>
            <p className="mt-2">No usamos tus datos para fines distintos a los que motivaron tu solicitud.</p>
          </Bloque>

          <Bloque titulo="¿Con quién compartimos tus datos?">
            <p>
              Tus datos no se venden ni se comparten con terceros para fines comerciales ajenos a nuestro servicio.
              Se almacenan en infraestructura de Microsoft Azure (Azure SQL Database), alojada en Estados Unidos,
              bajo los controles de seguridad estándar de esa plataforma (cifrado en tránsito y en reposo, acceso
              restringido por identidad administrada — sin contraseñas ni claves de acceso compartidas).
            </p>
          </Bloque>

          <Bloque titulo="¿Cuánto tiempo conservamos tus datos?">
            <p>
              Mientras exista una relación comercial contigo o mientras sea razonablemente necesario para el
              propósito que motivó la recolección — por ejemplo, mientras avances un proyecto con nosotros. Puedes
              solicitar la eliminación de tus datos en cualquier momento (ver más abajo).
            </p>
          </Bloque>

          <Bloque titulo="Tus derechos como titular de los datos">
            <p className="mb-2">De acuerdo con la Ley 1581 de 2012, tienes derecho a:</p>
            <ul className="list-disc space-y-1 pl-5">
              <li>Conocer, actualizar y rectificar tus datos personales.</li>
              <li>Solicitar prueba de la autorización que nos diste para tratarlos.</li>
              <li>Ser informado sobre el uso que les hemos dado.</li>
              <li>Revocar tu autorización y/o solicitar la supresión de tus datos, cuando no exista un deber legal o contractual que nos obligue a conservarlos.</li>
              <li>Presentar quejas ante la Superintendencia de Industria y Comercio (SIC) por infracciones a esta ley.</li>
            </ul>
          </Bloque>

          <Bloque titulo="¿Cómo ejerces estos derechos?">
            <p>
              Escríbenos por WhatsApp al{' '}
              <a href={`https://wa.me/${WHATSAPP_NUMBER}`} target="_blank" rel="noopener noreferrer" className="text-emerald-700 underline hover:text-emerald-800">
                +{WHATSAPP_NUMBER}
              </a>{' '}
              indicando tu solicitud (conocer, actualizar, rectificar o eliminar tus datos) y el correo electrónico
              con el que nos escribiste originalmente. Responderemos dentro de los términos que establece la ley.
            </p>
          </Bloque>

          <Bloque titulo="Cambios a esta política">
            <p>
              Podemos actualizar esta política ocasionalmente. La fecha de la versión vigente siempre está
              disponible en esta misma página.
            </p>
            <p className="mt-2 text-xs text-slate-400">Última actualización: 2 de septiembre de 2026.</p>
          </Bloque>
        </div>
      </main>
    </div>
  );
}

function Bloque({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <section className="rounded-xl border border-slate-200 bg-white p-5">
      <h2 className="mb-2 font-semibold text-slate-900">{titulo}</h2>
      <div className="text-sm text-slate-600">{children}</div>
    </section>
  );
}
