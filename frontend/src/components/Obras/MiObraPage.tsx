import { useEffect } from 'react';
import { useMiObra } from '../../hooks/useMiObra';
import { WhatsAppButton } from '../Properties/WhatsAppButton';
import { SITE_TITLE } from '../../seo';
import type { Hito } from '../../types/obras';

const ESTADO_PROYECTO_LABEL: Record<string, string> = {
  Planificacion: 'En planificación',
  EnEjecucion: 'En ejecución',
  Pausado: 'Pausado',
  Finalizado: 'Finalizado',
};

const ESTADO_HITO_LABEL: Record<string, string> = {
  Pendiente: 'Pendiente',
  EnProgreso: 'En progreso',
  Completado: 'Completado',
};

interface MiObraPageProps {
  token: string;
}

export function MiObraPage({ token }: MiObraPageProps) {
  const { proyecto, isLoading, notFound, error } = useMiObra(token);

  useEffect(() => {
    document.title = proyecto ? `${proyecto.nombreProyecto} — Avance de obra | Plataforma Civil e Inmobiliaria` : SITE_TITLE;
    return () => {
      document.title = SITE_TITLE;
    };
  }, [proyecto]);

  if (isLoading) {
    return <div className="mx-auto max-w-2xl px-6 py-16 text-center text-slate-500">Cargando…</div>;
  }

  if (notFound) {
    return (
      <div className="mx-auto max-w-2xl px-6 py-16 text-center">
        <h1 className="text-xl font-bold text-slate-900">Link no válido</h1>
        <p className="mt-2 text-slate-500">
          Este link de seguimiento de obra no existe o ya no está disponible. Verifica que lo copiaste completo, o
          pídele a tu asesor que te lo reenvíe.
        </p>
      </div>
    );
  }

  if (error || !proyecto) {
    return <div className="mx-auto max-w-2xl px-6 py-16 text-center text-red-600">{error ?? 'Error inesperado.'}</div>;
  }

  const totalHitos = proyecto.hitos.length;
  const hitosCompletados = proyecto.hitos.filter((h) => h.estado === 'Completado').length;
  const progreso = totalHitos > 0 ? Math.round((hitosCompletados / totalHitos) * 100) : 0;

  return (
    <div className="min-h-screen bg-gradient-to-b from-slate-100 to-white">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto max-w-3xl px-6 py-4">
          <span className="text-lg font-bold text-slate-900">
            Plataforma <span className="text-emerald-600">Civil &amp; Inmobiliaria</span>
          </span>
        </div>
      </header>

      <main className="mx-auto max-w-3xl px-6 py-10">
        <p className="text-sm text-slate-500">Hola {proyecto.nombreCliente.split(' ')[0]}, este es el avance de tu proyecto:</p>
        <h1 className="mt-1 text-2xl font-bold text-slate-900">{proyecto.nombreProyecto}</h1>
        {proyecto.descripcion && <p className="mt-2 text-slate-600">{proyecto.descripcion}</p>}

        <div className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
          <div className="mb-2 flex items-center justify-between">
            <span className="text-sm font-medium text-slate-700">{ESTADO_PROYECTO_LABEL[proyecto.estado] ?? proyecto.estado}</span>
            <span className="text-sm text-slate-500">
              {hitosCompletados} de {totalHitos} hitos completados
            </span>
          </div>
          <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
            <div className="h-full rounded-full bg-emerald-600 transition-all" style={{ width: `${progreso}%` }} />
          </div>
        </div>

        <div className="mt-8 flex flex-col gap-4">
          {proyecto.hitos.length === 0 ? (
            <p className="text-sm text-slate-500">Todavía no hay hitos registrados para este proyecto.</p>
          ) : (
            proyecto.hitos.map((hito) => <HitoCard key={hito.id} hito={hito} />)
          )}
        </div>

        <div className="mt-10 rounded-xl border border-slate-200 bg-white p-5">
          <p className="mb-3 text-sm text-slate-600">¿Tienes una pregunta sobre el avance de tu proyecto?</p>
          <WhatsAppButton mensaje={`Hola, tengo una pregunta sobre el avance de mi proyecto "${proyecto.nombreProyecto}".`} />
        </div>
      </main>
    </div>
  );
}

function HitoCard({ hito }: { hito: Hito }) {
  const completado = hito.estado === 'Completado';

  return (
    <div className={`flex gap-4 rounded-xl border p-4 ${completado ? 'border-emerald-200 bg-emerald-50' : 'border-slate-200 bg-white'}`}>
      <div className="flex flex-col items-center">
        <span
          className={`flex h-6 w-6 shrink-0 items-center justify-center rounded-full text-xs font-bold text-white ${
            completado ? 'bg-emerald-600' : hito.estado === 'EnProgreso' ? 'bg-blue-500' : 'bg-slate-300'
          }`}
        >
          {completado ? '✓' : ''}
        </span>
      </div>

      <div className="flex-1">
        <div className="flex items-center justify-between gap-2">
          <p className="font-medium text-slate-900">{hito.nombre}</p>
          <span className="text-xs font-medium text-slate-500">{ESTADO_HITO_LABEL[hito.estado] ?? hito.estado}</span>
        </div>
        {hito.descripcion && <p className="mt-1 text-sm text-slate-600">{hito.descripcion}</p>}
        {hito.fechaCompletado && (
          <p className="mt-1 text-xs text-slate-400">Completado el {new Date(hito.fechaCompletado).toLocaleDateString('es-CO')}</p>
        )}
        {hito.fotoEvidenciaUrl && (
          <img src={hito.fotoEvidenciaUrl} alt={hito.nombre} className="mt-3 h-40 w-full rounded-lg object-cover" />
        )}
      </div>
    </div>
  );
}
