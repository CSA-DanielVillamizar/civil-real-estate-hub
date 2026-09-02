interface PropertyLocationMapProps {
  latitud?: number;
  longitud?: number;
  titulo: string;
}

// Iframe de Google Maps embed — gratis, sin API key, sin librería nueva (ver
// decisión aprobada: coherente con la filosofía FinOps del proyecto, ver
// docs/02-business-case.md §6). Si la propiedad no tiene latitud/longitud
// cargada, no se muestra nada (no todas las propiedades las traen).
export function PropertyLocationMap({ latitud, longitud, titulo }: PropertyLocationMapProps) {
  if (latitud == null || longitud == null) return null;

  const src = `https://www.google.com/maps?q=${latitud},${longitud}&output=embed`;
  const comoLlegarHref = `https://www.google.com/maps/search/?api=1&query=${latitud},${longitud}`;

  return (
    <div>
      <div className="mb-2 flex items-center justify-between">
        <h2 className="font-semibold text-slate-900">Ubicación</h2>
        <a
          href={comoLlegarHref}
          target="_blank"
          rel="noopener noreferrer"
          className="text-sm text-emerald-700 hover:underline"
        >
          Cómo llegar ↗
        </a>
      </div>
      <iframe
        title={`Ubicación de ${titulo}`}
        src={src}
        className="h-72 w-full rounded-lg border border-slate-200"
        loading="lazy"
        referrerPolicy="no-referrer-when-downgrade"
      />
    </div>
  );
}
