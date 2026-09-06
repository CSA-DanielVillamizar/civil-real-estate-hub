import { useState, type FormEvent } from 'react';
import { useTarifasAdmin } from '../../hooks/useTarifasAdmin';
import type { AuthState } from '../../hooks/useAuth';
import { RolUsuario } from '../../types/auth';
import { ServicioDeInteres } from '../../types/common';
import type { PaqueteTarifa } from '../../types/tarifas';
import { AdminNav } from './AdminNav';
import { RequireAuth } from './RequireAuth';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

const SERVICIO_LABEL: Record<string, string> = {
  Inmobiliaria: 'Inmobiliaria',
  CalculadoraDeObra: 'Calculadora de obra',
  ConsultoriaYDisenoEstructural: 'Consultoría estructural',
  InterventoriaYPresupuestos: 'Interventoría y presupuestos',
};

function formatearPrecio(paquete: PaqueteTarifa): string {
  if (paquete.precioDesde == null && paquete.precioHasta == null) return 'Cotización personalizada';
  const fmt = (n: number) => n.toLocaleString('es-CO');
  if (paquete.precioDesde != null && paquete.precioHasta != null) {
    return `${paquete.moneda} ${fmt(paquete.precioDesde)} – ${fmt(paquete.precioHasta)} ${paquete.unidadPrecio}`;
  }
  const monto = paquete.precioDesde ?? paquete.precioHasta;
  return `Desde ${paquete.moneda} ${fmt(monto!)} ${paquete.unidadPrecio}`;
}

export function TarifasAdminPage() {
  return (
    <RequireAuth rolesPermitidos={[RolUsuario.Admin]}>
      {(auth, onUnauthorized) => <Panel auth={auth} onUnauthorized={onUnauthorized} />}
    </RequireAuth>
  );
}

function Panel({ auth, onUnauthorized }: { auth: AuthState; onUnauthorized: () => void }) {
  const { items, isLoading, error, fieldErrors, busyId, crear, actualizar, publicar, despublicar } = useTarifasAdmin(
    auth.token,
    onUnauthorized,
  );
  const [editandoId, setEditandoId] = useState<string | null>(null);

  return (
    <div>
      <AdminNav auth={auth} onLogout={onUnauthorized} />
      <div className="mx-auto max-w-5xl px-6 py-10">
        <h1 className="font-heading mb-1 text-2xl font-bold tracking-tight text-slate-900">Tarifas</h1>
        <p className="mb-6 text-sm text-slate-500">
          Transparencia de precios para consultoría estructural e interventoría — hoy no hay ningún indicio de costo
          en esas secciones. Nada se ve en el sitio hasta que lo publicas.
        </p>

        {error && <div className="mb-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        <div className="mb-8">
          <CrearPaqueteForm fieldErrors={fieldErrors} onCrear={crear} />
        </div>

        {!isLoading && items.length > 0 && <ResumenTarifas items={items} />}

        {isLoading ? (
          <p className="text-sm text-slate-500">Cargando…</p>
        ) : items.length === 0 ? (
          <p className="text-sm text-slate-500">Todavía no hay paquetes de tarifa.</p>
        ) : (
          <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
            <table className="w-full min-w-[820px] table-fixed border-collapse text-left text-sm">
              <thead className="bg-slate-50 text-left text-[11px] font-bold uppercase tracking-wide text-slate-500">
                <tr>
                  <th className="w-72 px-3 py-2">Paquete</th>
                  <th className="w-40 px-3 py-2">Servicio</th>
                  <th className="w-44 px-3 py-2">Precio</th>
                  <th className="w-28 px-3 py-2">Estado</th>
                  <th className="px-3 py-2 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {items.map((item) => (
                  <FilaPaquete
                    key={item.id}
                    item={item}
                    busy={busyId === item.id}
                    editando={editandoId === item.id}
                    fieldErrors={fieldErrors}
                    onToggleEditar={() => setEditandoId((id) => (id === item.id ? null : item.id))}
                    onActualizar={async (id, request) => {
                      const ok = await actualizar(id, request);
                      if (ok) setEditandoId(null);
                      return ok;
                    }}
                    onPublicar={publicar}
                    onDespublicar={despublicar}
                  />
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

// Tarjetas de resumen — agregados reales sobre los paquetes ya cargados. El
// conteo de "Servicios cubiertos" es la cantidad de servicios distintos
// (servicioRelacionado) que ya tienen al menos un paquete de tarifa.
function ResumenTarifas({ items }: { items: PaqueteTarifa[] }) {
  const publicados = items.filter((i) => i.publicado).length;
  const sinPublicar = items.length - publicados;
  const serviciosCubiertos = new Set(items.map((i) => i.servicioRelacionado)).size;

  const tarjetas = [
    { etiqueta: 'Total tarifas', valor: items.length },
    { etiqueta: 'Publicadas', valor: publicados },
    { etiqueta: 'Pendientes de publicar', valor: sinPublicar },
    { etiqueta: 'Servicios cubiertos', valor: serviciosCubiertos },
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

function FilaPaquete({
  item,
  busy,
  editando,
  fieldErrors,
  onToggleEditar,
  onActualizar,
  onPublicar,
  onDespublicar,
}: {
  item: PaqueteTarifa;
  busy: boolean;
  editando: boolean;
  fieldErrors: Record<string, string[]>;
  onToggleEditar: () => void;
  onActualizar: (
    id: string,
    request: { titulo: string; descripcion: string; precioDesde?: number; precioHasta?: number; unidadPrecio: string; servicioRelacionado: ServicioDeInteres },
  ) => Promise<boolean>;
  onPublicar: (id: string) => Promise<void>;
  onDespublicar: (id: string) => Promise<void>;
}) {
  return (
    <>
      <tr className="align-top hover:bg-slate-50">
        <td className="px-3 py-2">
          <p className="font-heading truncate font-semibold text-slate-900">{item.titulo}</p>
          <p className="truncate text-xs text-slate-500">{item.descripcion}</p>
        </td>
        <td className="px-3 py-2 text-slate-600">{SERVICIO_LABEL[item.servicioRelacionado] ?? item.servicioRelacionado}</td>
        <td className="font-heading px-3 py-2 text-slate-700">{formatearPrecio(item)}</td>
        <td className="px-3 py-2">
          <span
            className={`font-heading rounded-full px-2 py-1 text-[11px] font-bold uppercase tracking-wide ${
              item.publicado ? 'bg-emerald-100 text-emerald-800' : 'bg-amber-100 text-amber-800'
            }`}
          >
            {item.publicado ? 'Publicado' : 'Sin publicar'}
          </span>
        </td>
        <td className="px-3 py-2">
          <div className="flex flex-wrap items-center justify-end gap-1.5">
            <button
              type="button"
              onClick={onToggleEditar}
              className="rounded-md border border-slate-300 px-2 py-1 text-[11px] font-medium text-slate-700 hover:bg-slate-50"
            >
              {editando ? 'Cancelar' : 'Editar'}
            </button>
            {/* Publicar/Despublicar es un toggle reversible — se mantiene en
                gris neutro (no emerald/amber) para distinguirlo de acciones
                de un solo sentido como "Convertir" en Leads. */}
            <button
              type="button"
              onClick={() => (item.publicado ? onDespublicar(item.id) : onPublicar(item.id))}
              disabled={busy}
              className="rounded-md border border-slate-300 px-2 py-1 text-[11px] font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
            >
              {busy ? '…' : item.publicado ? 'Despublicar' : 'Publicar'}
            </button>
          </div>
        </td>
      </tr>
      {editando && (
        <tr className="bg-slate-50/60">
          <td colSpan={5} className="border-t border-slate-100 px-3 py-4">
            <EditarPaqueteForm
              item={item}
              fieldErrors={fieldErrors}
              onGuardar={(request) => onActualizar(item.id, request)}
            />
          </td>
        </tr>
      )}
    </>
  );
}

function EditarPaqueteForm({
  item,
  fieldErrors,
  onGuardar,
}: {
  item: PaqueteTarifa;
  fieldErrors: Record<string, string[]>;
  onGuardar: (request: {
    titulo: string;
    descripcion: string;
    precioDesde?: number;
    precioHasta?: number;
    unidadPrecio: string;
    servicioRelacionado: ServicioDeInteres;
  }) => Promise<boolean>;
}) {
  const [titulo, setTitulo] = useState(item.titulo);
  const [descripcion, setDescripcion] = useState(item.descripcion);
  const [precioDesde, setPrecioDesde] = useState(item.precioDesde?.toString() ?? '');
  const [precioHasta, setPrecioHasta] = useState(item.precioHasta?.toString() ?? '');
  const [unidadPrecio, setUnidadPrecio] = useState(item.unidadPrecio);
  const [servicioRelacionado, setServicioRelacionado] = useState<ServicioDeInteres>(item.servicioRelacionado as ServicioDeInteres);
  const [guardando, setGuardando] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setGuardando(true);
    await onGuardar({
      titulo,
      descripcion,
      precioDesde: precioDesde.trim() ? Number(precioDesde) : undefined,
      precioHasta: precioHasta.trim() ? Number(precioHasta) : undefined,
      unidadPrecio,
      servicioRelacionado,
    });
    setGuardando(false);
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3">
      <div>
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
      <div>
        <textarea
          value={descripcion}
          onChange={(e) => setDescripcion(e.target.value)}
          placeholder="Descripción"
          aria-label="Descripción"
          rows={3}
          className={inputClasses}
          required
        />
        {err('descripcion') && <p className="mt-1 text-xs text-red-600">{err('descripcion')}</p>}
      </div>
      <div className="grid grid-cols-2 gap-3">
        <input
          type="number"
          value={precioDesde}
          onChange={(e) => setPrecioDesde(e.target.value)}
          placeholder="Precio desde (opcional)"
          aria-label="Precio desde (opcional)"
          className={inputClasses}
        />
        <input
          type="number"
          value={precioHasta}
          onChange={(e) => setPrecioHasta(e.target.value)}
          placeholder="Precio hasta (opcional)"
          aria-label="Precio hasta (opcional)"
          className={inputClasses}
        />
      </div>
      <div>
        <input
          value={unidadPrecio}
          onChange={(e) => setUnidadPrecio(e.target.value)}
          placeholder='Unidad (ej. "por m²", "% del valor de la obra")'
          aria-label="Unidad de precio"
          className={inputClasses}
          required
        />
        {err('unidadPrecio') && <p className="mt-1 text-xs text-red-600">{err('unidadPrecio')}</p>}
      </div>
      <select
        value={servicioRelacionado}
        onChange={(e) => setServicioRelacionado(e.target.value as ServicioDeInteres)}
        aria-label="Servicio relacionado"
        className={inputClasses}
      >
        {Object.values(ServicioDeInteres).map((s) => (
          <option key={s} value={s}>
            {SERVICIO_LABEL[s]}
          </option>
        ))}
      </select>
      <button
        type="submit"
        disabled={guardando}
        className="font-heading justify-self-start rounded-md bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
      >
        {guardando ? 'Guardando…' : 'Guardar cambios'}
      </button>
    </form>
  );
}

function CrearPaqueteForm({
  fieldErrors,
  onCrear,
}: {
  fieldErrors: Record<string, string[]>;
  onCrear: (request: {
    servicioRelacionado: ServicioDeInteres;
    titulo: string;
    descripcion: string;
    precioDesde?: number;
    precioHasta?: number;
    unidadPrecio: string;
    moneda: string;
  }) => Promise<boolean>;
}) {
  const [servicioRelacionado, setServicioRelacionado] = useState<ServicioDeInteres>(ServicioDeInteres.ConsultoriaYDisenoEstructural);
  const [titulo, setTitulo] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [precioDesde, setPrecioDesde] = useState('');
  const [precioHasta, setPrecioHasta] = useState('');
  const [unidadPrecio, setUnidadPrecio] = useState('');
  const [creando, setCreando] = useState(false);
  const [creado, setCreado] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setCreando(true);
    setCreado(false);

    const ok = await onCrear({
      servicioRelacionado,
      titulo,
      descripcion,
      precioDesde: precioDesde.trim() ? Number(precioDesde) : undefined,
      precioHasta: precioHasta.trim() ? Number(precioHasta) : undefined,
      unidadPrecio,
      moneda: 'COP',
    });

    setCreando(false);
    if (ok) {
      setTitulo('');
      setDescripcion('');
      setPrecioDesde('');
      setPrecioHasta('');
      setUnidadPrecio('');
      setCreado(true);
    }
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5">
      <h3 className="font-heading mb-3 font-semibold text-slate-900">Nuevo paquete de tarifa</h3>

      {creado && (
        <div className="mb-4 rounded-md border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-900">
          Creado sin publicar. Revísalo en la lista de abajo y presiona "Publicar" cuando esté listo para el sitio.
        </div>
      )}

      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <select
          value={servicioRelacionado}
          onChange={(e) => setServicioRelacionado(e.target.value as ServicioDeInteres)}
          aria-label="Servicio relacionado"
          className={`${inputClasses} sm:col-span-2`}
        >
          {Object.values(ServicioDeInteres).map((s) => (
            <option key={s} value={s}>
              {SERVICIO_LABEL[s]}
            </option>
          ))}
        </select>

        <div className="sm:col-span-2">
          <input
            value={titulo}
            onChange={(e) => setTitulo(e.target.value)}
            placeholder="Nombre del paquete"
            aria-label="Nombre del paquete"
            className={inputClasses}
            required
          />
          {err('titulo') && <p className="mt-1 text-xs text-red-600">{err('titulo')}</p>}
        </div>

        <div className="sm:col-span-2">
          <textarea
            value={descripcion}
            onChange={(e) => setDescripcion(e.target.value)}
            placeholder="Qué incluye"
            aria-label="Qué incluye"
            rows={3}
            className={inputClasses}
            required
          />
          {err('descripcion') && <p className="mt-1 text-xs text-red-600">{err('descripcion')}</p>}
        </div>

        <input
          type="number"
          value={precioDesde}
          onChange={(e) => setPrecioDesde(e.target.value)}
          placeholder="Precio desde (opcional)"
          aria-label="Precio desde (opcional)"
          className={inputClasses}
        />
        <input
          type="number"
          value={precioHasta}
          onChange={(e) => setPrecioHasta(e.target.value)}
          placeholder="Precio hasta (opcional)"
          aria-label="Precio hasta (opcional)"
          className={inputClasses}
        />

        <div className="sm:col-span-2">
          <input
            value={unidadPrecio}
            onChange={(e) => setUnidadPrecio(e.target.value)}
            placeholder='Unidad (ej. "por m²", "% del valor de la obra", "tarifa plana")'
            aria-label="Unidad de precio"
            className={inputClasses}
            required
          />
          {err('unidadPrecio') && <p className="mt-1 text-xs text-red-600">{err('unidadPrecio')}</p>}
        </div>

        <button
          type="submit"
          disabled={creando}
          className="font-heading col-span-full rounded-md bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800 disabled:opacity-50"
        >
          {creando ? 'Creando…' : 'Crear (sin publicar)'}
        </button>
      </form>
    </div>
  );
}
