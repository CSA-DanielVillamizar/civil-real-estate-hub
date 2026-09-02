import { useEffect, useState } from 'react';
import type { ArchivoMultimedia } from '../../types/properties';

interface PhotoGalleryProps {
  fotos: ArchivoMultimedia[];
  titulo: string;
}

// Reemplaza la grilla estática anterior (sin interacción) por una grilla +
// lightbox: clic en cualquier foto la abre a pantalla completa, con
// navegación por flechas del teclado o los botones ‹/›. Sin librería nueva
// — es el mismo patrón de estado local que el resto de la app.
export function PhotoGallery({ fotos, titulo }: PhotoGalleryProps) {
  const [abiertaEn, setAbiertaEn] = useState<number | null>(null);

  useEffect(() => {
    if (abiertaEn === null) return;

    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') setAbiertaEn(null);
      if (e.key === 'ArrowRight') setAbiertaEn((i) => (i === null ? null : (i + 1) % fotos.length));
      if (e.key === 'ArrowLeft') setAbiertaEn((i) => (i === null ? null : (i - 1 + fotos.length) % fotos.length));
    }

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [abiertaEn, fotos.length]);

  if (fotos.length === 0) {
    return (
      <div className="mb-6 flex h-64 items-center justify-center rounded-lg bg-slate-100 text-slate-400">
        Sin fotos disponibles
      </div>
    );
  }

  return (
    <>
      <div className="mb-6 grid grid-cols-2 gap-2 sm:grid-cols-4">
        {fotos.map((foto, i) => (
          <button
            key={foto.id}
            type="button"
            onClick={() => setAbiertaEn(i)}
            className={`group relative overflow-hidden rounded-lg ${i === 0 ? 'col-span-2 row-span-2 h-full sm:col-span-2' : ''}`}
          >
            <img
              src={foto.url}
              alt={`${titulo} — foto ${i + 1}`}
              className="h-48 w-full object-cover transition group-hover:brightness-90 sm:h-full"
            />
          </button>
        ))}
      </div>

      {abiertaEn !== null && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/90 p-4"
          onClick={() => setAbiertaEn(null)}
          role="dialog"
          aria-modal="true"
          aria-label={`${titulo} — foto ${abiertaEn + 1} de ${fotos.length}`}
        >
          <button
            type="button"
            onClick={() => setAbiertaEn(null)}
            className="absolute right-4 top-4 text-3xl text-white/80 hover:text-white"
            aria-label="Cerrar"
          >
            ×
          </button>

          {fotos.length > 1 && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                setAbiertaEn((abiertaEn - 1 + fotos.length) % fotos.length);
              }}
              className="absolute left-4 text-4xl text-white/80 hover:text-white"
              aria-label="Foto anterior"
            >
              ‹
            </button>
          )}

          <img
            src={fotos[abiertaEn].url}
            alt={`${titulo} — foto ${abiertaEn + 1}`}
            className="max-h-[85vh] max-w-full rounded-lg object-contain"
            onClick={(e) => e.stopPropagation()}
          />

          {fotos.length > 1 && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                setAbiertaEn((abiertaEn + 1) % fotos.length);
              }}
              className="absolute right-4 text-4xl text-white/80 hover:text-white"
              aria-label="Foto siguiente"
            >
              ›
            </button>
          )}

          <p className="absolute bottom-4 text-sm text-white/70">
            {abiertaEn + 1} / {fotos.length}
          </p>
        </div>
      )}
    </>
  );
}
