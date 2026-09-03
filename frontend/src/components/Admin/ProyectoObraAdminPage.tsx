import { useRef, useState, type FormEvent } from 'react';
import { useProyectoObraAdmin } from '../../hooks/useProyectoObraAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import { EstadoHito, EstadoProyecto, type Hito } from '../../types/obras';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

const ESTADO_HITO_BADGE: Record<string, string> = {
  Pendiente: 'bg-slate-100 text-slate-700',
  EnProgreso: 'bg-blue-100 text-blue-800',
  Completado: 'bg-emerald-100 text-emerald-800',
};

interface ProyectoObraAdminPageProps {
  id: string;
}

export function ProyectoObraAdminPage({ id }: ProyectoObraAdminPageProps) {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <Panel id={id} auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ id, auth, onUnauthorized }: { id: string; auth: AuthState; onUnauthorized: () => void }) {
  const { proyecto, isLoading, error, busyHitoId, agregarHito, cambiarEstadoHito, subirEvidencia, cambiarEstadoProyecto } =
    useProyectoObraAdmin(id, auth.token, onUnauthorized);

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-3xl px-6 py-10">
        <a href="/admin/obras" className="mb-4 inline-block text-sm text-slate-500 hover:text-slate-900">
          ← Todos los proyectos
        </a>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        {isLoading && !proyecto ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : !proyecto ? (
          <p className="text-sm text-slate-500">Proyecto no encontrado.</p>
        ) : (
          <>
            <div className="mb-6 flex items-start justify-between gap-4">
              <div>
                <h1 className="text-2xl font-bold text-slate-900">{proyecto.nombreProyecto}</h1>
                <p className="text-sm text-slate-500">
                  {proyecto.nombreCliente} · {proyecto.emailCliente} · {proyecto.telefonoCliente}
                </p>
              </div>

              <select
                value={proyecto.estado}
                onChange={(e) => cambiarEstadoProyecto(e.target.value as EstadoProyecto)}
                aria-label="Estado del proyecto"
                className="rounded-md border border-slate-300 px-3 py-1.5 text-sm shadow-sm focus:border-emerald-500 focus:outline-none"
              >
                {Object.values(EstadoProyecto).map((estado) => (
                  <option key={estado} value={estado}>
                    {estado}
                  </option>
                ))}
              </select>
            </div>

            <h2 className="mb-3 font-semibold text-slate-900">Hitos</h2>

            <div className="mb-6 flex flex-col gap-3">
              {proyecto.hitos.length === 0 ? (
                <p className="text-sm text-slate-500">Aún no hay hitos.</p>
              ) : (
                proyecto.hitos.map((hito) => (
                  <HitoRow
                    key={hito.id}
                    hito={hito}
                    busy={busyHitoId === hito.id}
                    onCambiarEstado={(estado) => cambiarEstadoHito(hito.id, estado)}
                    onSubirEvidencia={(archivo) => subirEvidencia(hito.id, archivo)}
                  />
                ))
              )}
            </div>

            <NuevoHitoForm onAgregar={agregarHito} />
          </>
        )}
      </div>
    </div>
  );
}

function HitoRow({
  hito,
  busy,
  onCambiarEstado,
  onSubirEvidencia,
}: {
  hito: Hito;
  busy: boolean;
  onCambiarEstado: (estado: EstadoHito) => void;
  onSubirEvidencia: (archivo: File) => void;
}) {
  const fileInputRef = useRef<HTMLInputElement>(null);

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const archivo = e.target.files?.[0];
    if (archivo) onSubirEvidencia(archivo);
    e.target.value = '';
  }

  return (
    <div className="flex items-center justify-between gap-4 rounded-lg border border-slate-200 bg-white p-4">
      <div className="flex items-center gap-3">
        {hito.fotoEvidenciaUrl ? (
          <img src={hito.fotoEvidenciaUrl} alt="" className="h-14 w-14 rounded-md object-cover" />
        ) : (
          <div className="flex h-14 w-14 items-center justify-center rounded-md bg-slate-100 text-xs text-slate-400">Sin foto</div>
        )}
        <div>
          <p className="font-medium text-slate-900">{hito.nombre}</p>
          {hito.descripcion && <p className="text-xs text-slate-500">{hito.descripcion}</p>}
        </div>
      </div>

      <div className="flex items-center gap-2">
        <span className={`rounded-full px-2 py-1 text-xs font-medium ${ESTADO_HITO_BADGE[hito.estado] ?? 'bg-slate-100 text-slate-700'}`}>
          {hito.estado}
        </span>

        <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleFileChange} />
        <button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={busy}
          className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          {busy ? '…' : '+ Foto'}
        </button>

        {hito.estado === 'Pendiente' && (
          <button
            type="button"
            onClick={() => onCambiarEstado(EstadoHito.EnProgreso)}
            disabled={busy}
            className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
          >
            Iniciar
          </button>
        )}

        {hito.estado !== 'Completado' && (
          <button
            type="button"
            onClick={() => onCambiarEstado(EstadoHito.Completado)}
            disabled={busy}
            className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
          >
            Completar
          </button>
        )}
      </div>
    </div>
  );
}

function NuevoHitoForm({ onAgregar }: { onAgregar: (request: { nombre: string; descripcion?: string; fechaEstimada?: string }) => void }) {
  const [nombre, setNombre] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [fechaEstimada, setFechaEstimada] = useState('');

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!nombre.trim()) return;
    onAgregar({ nombre: nombre.trim(), descripcion: descripcion.trim() || undefined, fechaEstimada: fechaEstimada || undefined });
    setNombre('');
    setDescripcion('');
    setFechaEstimada('');
  }

  return (
    <form onSubmit={handleSubmit} className="rounded-xl border border-slate-200 bg-white p-5">
      <h3 className="mb-3 font-semibold text-slate-900">Agregar hito</h3>
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <input
          placeholder="Nombre del hito"
          aria-label="Nombre del hito"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
          className={inputClasses}
          required
        />
        <input
          type="date"
          value={fechaEstimada}
          onChange={(e) => setFechaEstimada(e.target.value)}
          aria-label="Fecha estimada (opcional)"
          className={inputClasses}
        />
        <textarea
          placeholder="Descripción (opcional)"
          aria-label="Descripción (opcional)"
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          className={`${inputClasses} col-span-full`}
          rows={2}
        />
        <button type="submit" className="col-span-full rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700">
          Agregar hito
        </button>
      </div>
    </form>
  );
}
