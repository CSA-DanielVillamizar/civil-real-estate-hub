const LINKS = [
  { href: '/admin/leads', label: 'Leads' },
  { href: '/admin/propiedades', label: 'Propiedades' },
  { href: '/admin/viabilidad-ambiental', label: 'Viabilidad ambiental' },
];

// Barra compartida entre las 3 pantallas administrativas — sin ella, cada
// una era una isla sin forma de llegar a las otras salvo escribiendo la URL
// a mano.
export function AdminNav() {
  const path = window.location.pathname;

  return (
    <div className="border-b border-slate-200 bg-white">
      <nav className="mx-auto flex max-w-6xl gap-1 px-6">
        {LINKS.map((link) => (
          <a
            key={link.href}
            href={link.href}
            className={`border-b-2 px-3 py-3 text-sm font-medium transition ${
              path === link.href
                ? 'border-emerald-600 text-emerald-700'
                : 'border-transparent text-slate-500 hover:text-slate-900'
            }`}
          >
            {link.label}
          </a>
        ))}
      </nav>
    </div>
  );
}
