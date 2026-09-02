import { useState, type FormEvent } from 'react';
import { TipoInmueble, TipoSuelo, Topografia, UnidadMedidaArea } from '../../types/common';
import type { CrearPropiedadRequest } from '../../types/properties';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

interface CrearPropiedadFormProps {
  fieldErrors: Record<string, string[]>;
  onCrear: (request: CrearPropiedadRequest) => Promise<string | null>;
}

const initial = {
  titulo: '',
  descripcion: '',
  tipoInmueble: TipoInmueble.Lote as string,
  precio: '',
  direccion: '',
  municipio: '',
  departamento: '',
  areaTerrenoValor: '',
  pendientePorcentaje: '',
  tipoSuelo: TipoSuelo.Franco as string,
  topografia: Topografia.Plana as string,
};

export function CrearPropiedadForm({ fieldErrors, onCrear }: CrearPropiedadFormProps) {
  const [values, setValues] = useState(initial);
  const [creando, setCreando] = useState(false);

  function set<K extends keyof typeof initial>(field: K, value: string) {
    setValues((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setCreando(true);

    const id = await onCrear({
      titulo: values.titulo,
      descripcion: values.descripcion,
      tipoInmueble: values.tipoInmueble as CrearPropiedadRequest['tipoInmueble'],
      precio: Number(values.precio),
      moneda: 'COP',
      direccion: values.direccion,
      municipio: values.municipio,
      departamento: values.departamento,
      areaTerrenoValor: Number(values.areaTerrenoValor),
      areaTerrenoUnidad: UnidadMedidaArea.M2,
      pendientePorcentaje: Number(values.pendientePorcentaje),
      tipoSuelo: values.tipoSuelo as CrearPropiedadRequest['tipoSuelo'],
      topografia: values.topografia as CrearPropiedadRequest['topografia'],
    });

    setCreando(false);
    if (id) setValues(initial);
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3 rounded-xl border border-slate-200 bg-white p-5 sm:grid-cols-2">
      <h3 className="col-span-full font-semibold text-slate-900">Nueva propiedad</h3>

      <div className="col-span-full">
        <input placeholder="Título" value={values.titulo} onChange={(e) => set('titulo', e.target.value)} className={inputClasses} required />
        {err('titulo') && <p className="mt-1 text-xs text-red-600">{err('titulo')}</p>}
      </div>

      <div className="col-span-full">
        <textarea
          placeholder="Descripción"
          value={values.descripcion}
          onChange={(e) => set('descripcion', e.target.value)}
          className={inputClasses}
          rows={2}
          required
        />
      </div>

      <select value={values.tipoInmueble} onChange={(e) => set('tipoInmueble', e.target.value)} className={inputClasses}>
        {Object.values(TipoInmueble).map((t) => (
          <option key={t} value={t}>
            {t}
          </option>
        ))}
      </select>

      <input
        type="number"
        placeholder="Precio (COP)"
        value={values.precio}
        onChange={(e) => set('precio', e.target.value)}
        className={inputClasses}
        required
      />

      <input placeholder="Dirección / referencia" value={values.direccion} onChange={(e) => set('direccion', e.target.value)} className={inputClasses} required />
      <input placeholder="Municipio" value={values.municipio} onChange={(e) => set('municipio', e.target.value)} className={inputClasses} required />
      <input placeholder="Departamento" value={values.departamento} onChange={(e) => set('departamento', e.target.value)} className={inputClasses} required />

      <input
        type="number"
        placeholder="Área terreno (m²)"
        value={values.areaTerrenoValor}
        onChange={(e) => set('areaTerrenoValor', e.target.value)}
        className={inputClasses}
        required
      />

      <input
        type="number"
        placeholder="Pendiente (%)"
        value={values.pendientePorcentaje}
        onChange={(e) => set('pendientePorcentaje', e.target.value)}
        className={inputClasses}
        required
      />

      <select value={values.tipoSuelo} onChange={(e) => set('tipoSuelo', e.target.value)} className={inputClasses}>
        {Object.values(TipoSuelo).map((t) => (
          <option key={t} value={t}>
            {t}
          </option>
        ))}
      </select>

      <select value={values.topografia} onChange={(e) => set('topografia', e.target.value)} className={inputClasses}>
        {Object.values(Topografia).map((t) => (
          <option key={t} value={t}>
            {t}
          </option>
        ))}
      </select>

      <button
        type="submit"
        disabled={creando}
        className="col-span-full mt-2 rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
      >
        {creando ? 'Creando…' : 'Crear propiedad (Borrador)'}
      </button>
    </form>
  );
}
