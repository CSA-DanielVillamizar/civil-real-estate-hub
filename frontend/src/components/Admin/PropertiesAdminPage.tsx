import { useRef, useState, type FormEvent } from 'react';
import { useAdminApiKey } from '../../hooks/useAdminApiKey';
import { usePropertiesAdmin } from '../../hooks/usePropertiesAdmin';
import { TipoMultimedia } from '../../types/common';
import type { PropertyResponse } from '../../types/properties';
import { CrearPropiedadForm } from './CrearPropiedadForm';

const ESTADO_BADGE: Record<string, string> = {
  Borrador: 'bg-slate-100 text-slate-700',
  Publicada: 'bg-emerald-100 text-emerald-800',
  Reservada: 'bg-amber-100 text-amber-800',
  Vendida: 'bg-blue-100 text-blue-800',
  Arrendada: 'bg-blue-100 text-blue-800',
  Retirada: 'bg-red-100 text-red-800',
};

export function PropertiesAdminPage() {
  const { apiKey, guardar, limpiar } = useAdminApiKey();

  if (!apiKey) return <ApiKeyGate onGuardar={guardar} />;
  return <Panel apiKey={apiKey} onUnauthorized={limpiar} />;
}

function ApiKeyGate({ onGuardar }: { onGuardar: (apiKey: string) => void }) {
  const [valor, setValor] = useState('');

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (valor.trim()) onGuardar(valor.trim());
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-sm flex-col justify-center px-6">
      <h1 className="mb-2 text-xl font-bold text-slate-900">Panel administrativo</h1>
      <p className="mb-6 text-sm text-slate-500">Ingresa el API key de administrador.</p>
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <input
          type="password"
          value={valor}
          onChange={(e) => setValor(e.target.value)}
          placeholder="X-Admin-Api-Key"
          className="rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-emerald-500 focus:outline-none"
          autoFocus
        />
        <button type="submit" disabled={!valor.trim()} className="rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50">
          Entrar
        </button>
      </form>
    </div>
  );
}

function Panel({ apiKey, onUnauthorized }: { apiKey: string; onUnauthorized: () => void }) {
  const { properties, isLoading, error, fieldErrors, busyId, crear, subirFoto, publicar } = usePropertiesAdmin(apiKey, onUnauthorized);

  return (
    <div className="mx-auto max-w-4xl px-6 py-10">
      <h1 className="mb-1 text-2xl font-bold text-slate-900">Propiedades</h1>
      <p className="mb-6 text-sm text-slate-500">Crea, sube fotos y publica propiedades en el catálogo.</p>

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
            <PropertyRow key={p.id} property={p} busy={busyId === p.id} onSubirFoto={subirFoto} onPublicar={publicar} />
          ))}
        </div>
      )}
    </div>
  );
}

function PropertyRow({
  property,
  busy,
  onSubirFoto,
  onPublicar,
}: {
  property: PropertyResponse;
  busy: boolean;
  onSubirFoto: (id: string, archivo: File, tipo: string) => void;
  onPublicar: (id: string) => void;
}) {
  const fileInputRef = useRef<HTMLInputElement>(null);

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const archivo = e.target.files?.[0];
    if (archivo) onSubirFoto(property.id, archivo, TipoMultimedia.Foto);
    e.target.value = '';
  }

  return (
    <div className="flex items-center justify-between gap-4 rounded-lg border border-slate-200 bg-white p-4">
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

      <div className="flex items-center gap-2">
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

        {property.estado === 'Borrador' && (
          <button
            type="button"
            onClick={() => onPublicar(property.id)}
            disabled={busy}
            className="rounded-md bg-emerald-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
          >
            Publicar
          </button>
        )}
      </div>
    </div>
  );
}
