// Google Analytics 4 (gtag.js) — carga condicional (gap #7). Sin
// VITE_GA_MEASUREMENT_ID configurado (no lo está todavía: requiere que el
// negocio cree su propia propiedad GA4 en analytics.google.com), esta
// función no hace nada — cero scripts de terceros, cero cookies de
// tracking. Se activa el día que exista un Measurement ID real, inyectando
// la variable de entorno de build VITE_GA_MEASUREMENT_ID (mismo patrón que
// VITE_SITE_URL/VITE_API_BASE_URL en .github/workflows/frontend-deploy.yml)
// — sin volver a tocar código.
//
// Importante: el día que se active, hay que sumar una mención a
// analytics/cookies en PoliticaPrivacidadPage.tsx (Habeas Data) — hoy esa
// página no la incluye porque hoy no hay ningún tracking real que declarar.

declare global {
  interface Window {
    dataLayer?: unknown[];
    gtag?: (...args: unknown[]) => void;
  }
}

export function initAnalytics(): void {
  const measurementId = import.meta.env.VITE_GA_MEASUREMENT_ID;
  if (!measurementId) return;

  const script = document.createElement('script');
  script.async = true;
  script.src = `https://www.googletagmanager.com/gtag/js?id=${measurementId}`;
  document.head.appendChild(script);

  window.dataLayer = window.dataLayer ?? [];
  function gtag(...args: unknown[]) {
    window.dataLayer!.push(args);
  }
  window.gtag = gtag;

  gtag('js', new Date());
  // anonymize_ip: minimiza el dato personal que se envía a Google mientras
  // no exista todavía un banner de consentimiento de cookies en el sitio.
  gtag('config', measurementId, { anonymize_ip: true });
}
