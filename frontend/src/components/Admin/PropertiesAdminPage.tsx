import { useEffect, useRef, useState } from 'react';
import { usePropertiesAdmin } from '../../hooks/usePropertiesAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { getPropertyById } from '../../services/propertiesService';
import { RolUsuario } from '../../types/auth';
import { TipoMultimedia } from '../../types/common';
import type { ActualizarDatosBasicosPropiedadRequest, PropertyResponse } from '../../types/properties';
import { CrearPropiedadForm } from './CrearPropiedadForm';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

const ESTADO_BADGE: Record<string, string> = {
  Borrador: 'bg-slate-100 text-slate-700',
  Publicada: 'bg-emerald-100 text-emerald-800',
  Reservada: 'bg-amber-100 text-amber-800',
  Vendida: 'bg-blue-100 text-blue-800',
  Arrendada: 'bg-blue-100 text-blue-800',
  Retirada: 'bg-red-100 text-red-800',
};

export function PropertiesAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <Panel auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const {
    properties,
    isLoading,
    error,
    fieldErrors,
    busyId,
    crear,
    subirFoto,
    publicar,
    reservar,
    marcarVendida,
    marcarArrendada,
    retirar,
    actualizarDatosBasicos,
  } = usePropertiesAdmin(auth.token, onUnauthorized);

  return (
    <div>
    <AdminNav auth={auth} onLogout={onUnauthorized} />
    <div className="mx-auto max-w-4xl px-6 py-10">
      <h1 className="mb-1 text-2xl font-bold text-slate-900">Propiedades</h1>
      <p className="mb-6 text-sm text-slate-500">Crea, edita, sube fotos y gestiona el estado de cada propiedad.</p>

      {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <div className="mb-8">
        <CrearPropiedadForm fieldErrors={fieldErrors} onCrear={crear} />
      </div>

      {isLoading ? (
        <p className="text-sm text-slate-500">Cargando…</p>
      ) : properties.length === 0 ? (
        <p className="text-sm text-slate-500">Aún no hay propiedades.</p>
      ) : (
        <div className="flex flex-col gap-3">
          {properties.map((p) => (
            <PropertyRow
              key={p.id}
              property={p}
              busy={busyId === p.id}
              fieldErrors={fieldErrors}
              onSubirFoto={subirFoto}
              onPublicar={publicar}
              onReservar={reservar}
              onMarcarVendida={marcarVendida}
              onMarcarArrendada={marcarArrendada}
              onRetirar={retirar}
              onActualizarDatosBasicos={actualizarDatosBasicos}
            />
          ))}
        </div>
      )}
    </div>
    </div>
  );
}

interface PropertyRowProps {
  property: PropertyResponse;
  busy: boolean;
  fieldErrors: Record<string, string[]>;
  onSubirFoto: (id: string, archivo: File, tipo: TipoMultimedia) => Promise<void>;
  onPublicar: (id: string) => Promise<void>;
  onReservar: (id: string) => Promise<void>;
  onMarcarVendida: (id: string) => Promise<void>;
  onMarcarArrendada: (id: string) => Promise<void>;
  onRetirar: (id: string) => Promise<void>;
  onActualizarDatosBasicos: (id: string, request: ActualizarDatosBasicosPropiedadRequest) => Promise<boolean>;
}

function PropertyRow({
  property,
  busy,
  fieldErrors,
  onSubirFoto,
  onPublicar,
  onReservar,
  onMarcarVendida,
  onMarcarArrendada,
  onRetirar,
  onActualizarDatosBasicos,
}: PropertyRowProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [editando, setEditando] = useState(false);

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const archivo = e.target.files?.[0];
    if (archivo) onSubirFoto(property.id, archivo, TipoMultimedia.Foto);
    e.target.value = '';
  }

  return (
    <div className="rounded-lg border border-slate-200 bg-white p-4">
      <div className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          {property.fotoPrincipalUrl ? (
            <img src={property.fotoPrincipalUrl} alt="" className="h-14 w-14 rounded-md object-cover" />
          ) : (
            <div className="flex h-14 w-14 items-center justify-center rounded-md bg-slate-100 text-xs text-slate-400">Sin foto</div>
          )}
          <div>
            <p className="font-medium text-slate-900">{property.titulo}</p>
            <p className="text-xs text-slate-500">
              {property.municipio}, {property.departamento} · {property.precio.toLocaleString('es-CO')} {property.moneda}
            </p>
          </div>
        </div>

        <div className="flex flex-wrap items-center justify-end gap-2">
          <span className={`rounded-full px-2 py-1 text-xs font-medium ${ESTADO_BADGE[property.estado] ?? 'bg-slate-100 text-slate-700'}`}>
            {property.estado}
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

          <button
            type="button"
            onClick={() => setEditando((v) => !v)}
            className="rounded-md border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 hover:bg-slate-50"
          >
            {editando ? 'Cancelar' : 'Editar'}
          </button>

          {(property.estado === 'Borrador' || property.estado === 'Retirada') && (
            <button
              type="button"
              onClick={() => onPublicar(property.id)}
              disabled={busy}
              className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
            >
              Publicar
            </button>
          )}

          {property.estado === 'Publicada' && (
            <button
              type="button"
              onClick={() => onReservar(property.id)}
              disabled={busy}
              className="rounded-md border border-amber-300 px-3 py-1.5 text-xs font-medium text-amber-800 hover:bg-amber-50 disabled:opacity-50"
            >
              Reservar
            </button>
          )}

          {(property.estado === 'Publicada' || property.estado === 'Reservada') && (
            <>
              <button
                type="button"
                onClick={() => onMarcarVendida(property.id)}
                disabled={busy}
                className="rounded-md border border-blue-300 px-3 py-1.5 text-xs font-medium text-blue-800 hover:bg-blue-50 disabled:opacity-50"
              >
                Marcar vendida
              </button>
              <button
                type="button"
                onClick={() => onMarcarArrendada(property.id)}
                disabled={busy}
                className="rounded-md border border-blue-300 px-3 py-1.5 text-xs font-medium text-blue-800 hover:bg-blue-50 disabled:opacity-50"
              >
                Marcar arrendada
              </button>
            </>
          )}

          {(property.estado === 'Publicada' || property.estado === 'Reservada' || property.estado === 'Arrendada') && (
            <button
              type="button"
              onClick={() => onRetirar(property.id)}
              disabled={busy}
              className="rounded-md border border-red-200 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
            >
              Retirar
            </button>
          )}
        </div>
      </div>

      {editando && (
        <EditarPropiedadForm
          propiedadId={property.id}
          fieldErrors={fieldErrors}
          onGuardar={async (request) => {
            const ok = await onActualizarDatosBasicos(property.id, request);
            if (ok) setEditando(false);
          }}
          onCancelar={() => setEditando(false)}
        />
      )}
    </div>
  );
}

function EditarPropiedadForm({
  propiedadId,
  fieldErrors,
  onGuardar,
  onCancelar,
}: {
  propiedadId: string;
  fieldErrors: Record<string, string[]>;
  onGuardar: (request: ActualizarDatosBasicosPropiedadRequest) => Promise<void>;
  onCancelar: () => void;
}) {
  const [cargando, setCargando] = useState(true);
  const [guardando, setGuardando] = useState(false);
  const [titulo, setTitulo] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [precio, setPrecio] = useState('');
  const [moneda, setMoneda] = useState('COP');

  useEffect(() => {
    const controller = new AbortController();
    getPropertyById(propiedadId, controller.signal)
      .then((detalle) => {
        setTitulo(detalle.titulo);
        setDescripcion(detalle.descripcion);
        setPrecio(String(detalle.precio));
        setMoneda(detalle.moneda);
      })
      .finally(() => setCargando(false));
    return () => controller.abort();
  }, [propiedadId]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setGuardando(true);
    await onGuardar({ titulo, descripcion, precio: Number(precio), moneda });
    setGuardando(false);
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  if (cargando) {
    return <p className="mt-3 text-sm text-slate-500">Cargando datos…</p>;
  }

  return (
    <form onSubmit={handleSubmit} className="mt-4 grid grid-cols-1 gap-3 border-t border-slate-100 pt-4 sm:grid-cols-2">
      <div className="col-span-full">
        <input
          value={titulo}
          onChange={(e) => setTitulo(e.target.value)}
          placeholder="Título"
          aria-label="Título"
          className={inputClasses}
          required
        />
        {err('titulo') && <p className="mt-1 text-xs text-red-600">{err('titulo')}</p>}
      </div>
      <div className="col-span-full">
        <textarea
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          placeholder="Descripción"
          aria-label="Descripción"
          className={inputClasses}
          rows={3}
          required
        />
        {err('descripcion') && <p className="mt-1 text-xs text-red-600">{err('descripcion')}</p>}
      </div>
      <div>
        <input
          type="number"
          value={precio}
          onChange={(e) => setPrecio(e.target.value)}
          placeholder="Precio"
          aria-label="Precio"
          className={inputClasses}
          required
        />
        {err('precio') && <p className="mt-1 text-xs text-red-600">{err('precio')}</p>}
      </div>
      <input
        value={moneda}
        onChange={(e) => setMoneda(e.target.value)}
        placeholder="Moneda"
        aria-label="Moneda"
        className={inputClasses}
        required
      />

      <div className="col-span-full flex gap-2">
        <button
          type="submit"
          disabled={guardando}
          className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
        >
          {guardando ? 'Guardando…' : 'Guardar cambios'}
        </button>
        <button type="button" onClick={onCancelar} className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50">
          Cancelar
        </button>
      </div>
    </form>
  );
}
