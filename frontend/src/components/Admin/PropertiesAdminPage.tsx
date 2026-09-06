import { useEffect, useRef, useState } from 'react';
import { usePropertiesAdmin } from '../../hooks/usePropertiesAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { getPropertyById } from '../../services/propertiesService';
import { RolUsuario } from '../../types/auth';
import { EstadoPropiedad, TipoMultimedia } from '../../types/common';
import type { ActualizarDatosBasicosPropiedadRequest, PropertyResponse } from '../../types/properties';
import { AlertTriangleIcon, CheckCircleIcon } from '../common/icons';
import { CrearPropiedadForm } from './CrearPropiedadForm';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

const ESTADO_BADGE: Record<string, string> = {
  Borrador: 'bg-slate-100 text-slate-700',
  Publicada: 'bg-emerald-100 text-emerald-800',
  Reservada: 'bg-amber-100 text-amber-800',
  Vendida: 'bg-sky-100 text-sky-800',
  Arrendada: 'bg-sky-100 text-sky-800',
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
      <div className="mx-auto max-w-6xl px-6 py-10">
        <h1 className="font-heading mb-1 text-2xl font-bold tracking-tight text-slate-900">Propiedades</h1>
        <p className="mb-6 text-sm text-slate-500">Crea, edita, sube fotos y gestiona el estado de cada propiedad.</p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        {!isLoading && properties.length > 0 && <ResumenPropiedades properties={properties} />}

        <div className="mb-8">
          <CrearPropiedadForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : properties.length === 0 ? (
          <p className="text-sm text-slate-500">Aún no hay propiedades.</p>
        ) : (
          <TablaPropiedades
            properties={properties}
            busyId={busyId}
            fieldErrors={fieldErrors}
            onSubirFoto={subirFoto}
            onPublicar={publicar}
            onReservar={reservar}
            onMarcarVendida={marcarVendida}
            onMarcarArrendada={marcarArrendada}
            onRetirar={retirar}
            onActualizarDatosBasicos={actualizarDatosBasicos}
          />
        )}
      </div>
    </div>
  );
}

// Tarjetas de resumen — agregados 100% reales sobre las propiedades ya
// cargadas (conteos por estado + viabilidad), nada inventado.
function ResumenPropiedades({ properties }: { properties: PropertyResponse[] }) {
  const publicadas = properties.filter((p) => p.estado === EstadoPropiedad.Publicada).length;
  const reservadas = properties.filter((p) => p.estado === EstadoPropiedad.Reservada).length;
  const conRestricciones = properties.filter((p) => !p.esViableConstructivamente).length;

  const tarjetas = [
    { etiqueta: 'Total propiedades', valor: properties.length },
    { etiqueta: 'Publicadas', valor: publicadas },
    { etiqueta: 'Reservadas', valor: reservadas },
    { etiqueta: 'Con restricciones', valor: conRestricciones },
  ];

  return (
    <div className="mb-6 grid grid-cols-2 gap-3 sm:grid-cols-4">
      {tarjetas.map((t) => (
        <div key={t.etiqueta} className="rounded-xl border border-slate-200 bg-white p-4">
          <p className="font-heading text-[11px] font-bold uppercase tracking-wide text-slate-500">{t.etiqueta}</p>
          <p className="font-heading mt-1 text-2xl font-bold tracking-tight text-slate-900">{t.valor}</p>
        </div>
      ))}
    </div>
  );
}

interface TablaPropiedadesProps {
  properties: PropertyResponse[];
  busyId: string | null;
  fieldErrors: Record<string, string[]>;
  onSubirFoto: (id: string, archivo: File, tipo: TipoMultimedia) => Promise<void>;
  onPublicar: (id: string) => Promise<void>;
  onReservar: (id: string) => Promise<void>;
  onMarcarVendida: (id: string) => Promise<void>;
  onMarcarArrendada: (id: string) => Promise<void>;
  onRetirar: (id: string) => Promise<void>;
  onActualizarDatosBasicos: (id: string, request: ActualizarDatosBasicosPropiedadRequest) => Promise<boolean>;
}

// Tabla de alta densidad (8pt sub-grid: celdas con py-2/px-3) — reemplaza la
// lista de tarjetas anterior. La columna "Viabilidad" es el dato que pide
// ser escaneable a simple vista: ícono + color, sin tener que leer texto.
function TablaPropiedades({
  properties,
  busyId,
  fieldErrors,
  onSubirFoto,
  onPublicar,
  onReservar,
  onMarcarVendida,
  onMarcarArrendada,
  onRetirar,
  onActualizarDatosBasicos,
}: TablaPropiedadesProps) {
  const [editandoId, setEditandoId] = useState<string | null>(null);

  return (
    <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
      <table className="w-full min-w-[900px] table-fixed border-collapse text-left text-sm">
        <thead className="bg-slate-50 text-left text-[11px] font-bold uppercase tracking-wide text-slate-500">
          <tr>
            <th className="w-64 px-3 py-2">Propiedad</th>
            <th className="w-24 px-3 py-2">Tipo</th>
            <th className="w-32 px-3 py-2">Precio</th>
            <th className="w-28 px-3 py-2">Área</th>
            <th className="w-24 px-3 py-2">Estado</th>
            <th className="w-28 px-3 py-2">Viabilidad</th>
            <th className="px-3 py-2 text-right">Acciones</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {properties.map((p) => (
            <FilaPropiedad
              key={p.id}
              property={p}
              busy={busyId === p.id}
              editando={editandoId === p.id}
              fieldErrors={fieldErrors}
              onToggleEditar={() => setEditandoId((actual) => (actual === p.id ? null : p.id))}
              onSubirFoto={onSubirFoto}
              onPublicar={onPublicar}
              onReservar={onReservar}
              onMarcarVendida={onMarcarVendida}
              onMarcarArrendada={onMarcarArrendada}
              onRetirar={onRetirar}
              onGuardar={async (request) => {
                const ok = await onActualizarDatosBasicos(p.id, request);
                if (ok) setEditandoId(null);
              }}
            />
          ))}
        </tbody>
      </table>
    </div>
  );
}

function FilaPropiedad({
  property,
  busy,
  editando,
  fieldErrors,
  onToggleEditar,
  onSubirFoto,
  onPublicar,
  onReservar,
  onMarcarVendida,
  onMarcarArrendada,
  onRetirar,
  onGuardar,
}: {
  property: PropertyResponse;
  busy: boolean;
  editando: boolean;
  fieldErrors: Record<string, string[]>;
  onToggleEditar: () => void;
  onSubirFoto: (id: string, archivo: File, tipo: TipoMultimedia) => Promise<void>;
  onPublicar: (id: string) => Promise<void>;
  onReservar: (id: string) => Promise<void>;
  onMarcarVendida: (id: string) => Promise<void>;
  onMarcarArrendada: (id: string) => Promise<void>;
  onRetirar: (id: string) => Promise<void>;
  onGuardar: (request: ActualizarDatosBasicosPropiedadRequest) => Promise<void>;
}) {
  const fileInputRef = useRef<HTMLInputElement>(null);

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const archivo = e.target.files?.[0];
    if (archivo) onSubirFoto(property.id, archivo, TipoMultimedia.Foto);
    e.target.value = '';
  }

  return (
    <>
      <tr className="align-top hover:bg-slate-50">
        <td className="px-3 py-2">
          <div className="flex items-center gap-2">
            {property.fotoPrincipalUrl ? (
              <img src={property.fotoPrincipalUrl} alt="" className="h-10 w-10 shrink-0 rounded-md object-cover" />
            ) : (
              <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-md bg-slate-100 text-[10px] text-slate-400">
                Sin foto
              </div>
            )}
            <div className="min-w-0">
              <p className="font-heading truncate font-semibold text-slate-900">{property.titulo}</p>
              <p className="truncate text-xs text-slate-500">
                {property.municipio}, {property.departamento}
              </p>
            </div>
          </div>
        </td>
        <td className="px-3 py-2 text-slate-600">{property.tipoInmueble}</td>
        <td className="font-heading px-3 py-2 font-medium text-slate-900">
          {property.precio.toLocaleString('es-CO')} {property.moneda}
        </td>
        <td className="font-heading px-3 py-2 text-slate-600">
          {property.areaTerrenoM2.toLocaleString('es-CO')} m²
          {property.areaConstruidaM2 ? (
            <span className="block text-xs text-slate-400">{property.areaConstruidaM2.toLocaleString('es-CO')} m² const.</span>
          ) : null}
        </td>
        <td className="px-3 py-2">
          <span className={`font-heading rounded-full px-2 py-1 text-[11px] font-bold uppercase tracking-wide ${ESTADO_BADGE[property.estado] ?? 'bg-slate-100 text-slate-700'}`}>
            {property.estado}
          </span>
        </td>
        <td className="px-3 py-2">
          {property.esViableConstructivamente ? (
            <span className="flex items-center gap-1 text-xs font-semibold text-emerald-700">
              <CheckCircleIcon className="h-4 w-4 shrink-0" /> Viable
            </span>
          ) : (
            <span className="flex items-center gap-1 text-xs font-semibold text-amber-700">
              <AlertTriangleIcon className="h-4 w-4 shrink-0" /> Restricciones
            </span>
          )}
        </td>
        <td className="px-3 py-2">
          <div className="flex flex-wrap items-center justify-end gap-1.5">
            <input ref={fileInputRef} type="file" accept="image/*" className="hidden" onChange={handleFileChange} />
            <button
              type="button"
              onClick={() => fileInputRef.current?.click()}
              disabled={busy}
              className="rounded-md border border-slate-300 px-2 py-1 text-[11px] font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
            >
              {busy ? '…' : '+ Foto'}
            </button>

            <button
              type="button"
              onClick={onToggleEditar}
              className="rounded-md border border-slate-300 px-2 py-1 text-[11px] font-medium text-slate-700 hover:bg-slate-50"
            >
              {editando ? 'Cancelar' : 'Editar'}
            </button>

            {(property.estado === 'Borrador' || property.estado === 'Retirada') && (
              <button
                type="button"
                onClick={() => onPublicar(property.id)}
                disabled={busy}
                className="rounded-md bg-emerald-600 px-2 py-1 text-[11px] font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
              >
                Publicar
              </button>
            )}

            {property.estado === 'Publicada' && (
              <button
                type="button"
                onClick={() => onReservar(property.id)}
                disabled={busy}
                className="rounded-md border border-amber-300 px-2 py-1 text-[11px] font-medium text-amber-800 hover:bg-amber-50 disabled:opacity-50"
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
                  className="rounded-md border border-sky-300 px-2 py-1 text-[11px] font-medium text-sky-800 hover:bg-sky-50 disabled:opacity-50"
                >
                  Vendida
                </button>
                <button
                  type="button"
                  onClick={() => onMarcarArrendada(property.id)}
                  disabled={busy}
                  className="rounded-md border border-sky-300 px-2 py-1 text-[11px] font-medium text-sky-800 hover:bg-sky-50 disabled:opacity-50"
                >
                  Arrendada
                </button>
              </>
            )}

            {(property.estado === 'Publicada' || property.estado === 'Reservada' || property.estado === 'Arrendada') && (
              <button
                type="button"
                onClick={() => onRetirar(property.id)}
                disabled={busy}
                className="rounded-md border border-red-200 px-2 py-1 text-[11px] font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
              >
                Retirar
              </button>
            )}
          </div>
        </td>
      </tr>

      {editando && (
        <tr>
          <td colSpan={7} className="border-t border-slate-100 bg-slate-50 px-3 py-4">
            <EditarPropiedadForm propiedadId={property.id} fieldErrors={fieldErrors} onGuardar={onGuardar} onCancelar={onToggleEditar} />
          </td>
        </tr>
      )}
    </>
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
    return <p className="text-sm text-slate-500">Cargando datos…</p>;
  }

  return (
    <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3 sm:grid-cols-2">
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
          className="font-heading rounded-md bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
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
