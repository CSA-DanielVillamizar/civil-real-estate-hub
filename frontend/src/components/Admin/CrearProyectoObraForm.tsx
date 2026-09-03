import { useState, type FormEvent } from 'react';
import type { CrearProyectoObraRequest } from '../../types/obras';

const inputClasses =
  'w-full rounded-md border border-slate-300 px-3 py-2 text-sm shadow-sm outline-none focus:border-emerald-500 focus:ring-2 focus:ring-emerald-500/40';

interface CrearProyectoObraFormProps {
  fieldErrors: Record<string, string[]>;
  onCrear: (request: CrearProyectoObraRequest) => Promise<{ id: string; tokenAcceso: string } | null>;
}

const initial = {
  nombreCliente: '',
  emailCliente: '',
  telefonoCliente: '',
  nombreProyecto: '',
  descripcion: '',
};

export function CrearProyectoObraForm({ fieldErrors, onCrear }: CrearProyectoObraFormProps) {
  const [values, setValues] = useState(initial);
  const [creando, setCreando] = useState(false);
  const [linkCreado, setLinkCreado] = useState<string | null>(null);

  function set<K extends keyof typeof initial>(field: K, value: string) {
    setValues((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setCreando(true);
    setLinkCreado(null);

    const resultado = await onCrear({
      nombreCliente: values.nombreCliente,
      emailCliente: values.emailCliente,
      telefonoCliente: values.telefonoCliente,
      nombreProyecto: values.nombreProyecto,
      descripcion: values.descripcion || undefined,
    });

    setCreando(false);
    if (resultado) {
      setValues(initial);
      setLinkCreado(`${window.location.origin}/mi-obra/${resultado.tokenAcceso}`);
    }
  }

  const err = (field: string) => fieldErrors[field]?.[0];

  return (
    <div className="rounded-xl border border-slate-200 bg-white p-5">
      <h3 className="mb-3 font-semibold text-slate-900">Nuevo proyecto</h3>

      {linkCreado && (
        <div className="mb-4 rounded-md border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-900">
          <p className="mb-1 font-medium">Proyecto creado. Comparte este link con el cliente:</p>
          <div className="flex items-center gap-2">
            <code className="flex-1 truncate rounded bg-white px-2 py-1 text-xs">{linkCreado}</code>
            <button
              type="button"
              onClick={() => navigator.clipboard.writeText(linkCreado)}
              className="rounded-md bg-emerald-600 px-2 py-1 text-xs font-semibold text-white hover:bg-emerald-700"
            >
              Copiar
            </button>
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit} className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <div>
          <input
            placeholder="Nombre del cliente"
            aria-label="Nombre del cliente"
            value={values.nombreCliente}
            onChange={(e) => set('nombreCliente', e.target.value)}
            className={inputClasses}
            required
          />
          {err('nombreCliente') && <p className="mt-1 text-xs text-red-600">{err('nombreCliente')}</p>}
        </div>

        <div>
          <input
            type="email"
            placeholder="Email del cliente"
            aria-label="Email del cliente"
            value={values.emailCliente}
            onChange={(e) => set('emailCliente', e.target.value)}
            className={inputClasses}
            required
          />
          {err('emailCliente') && <p className="mt-1 text-xs text-red-600">{err('emailCliente')}</p>}
        </div>

        <div>
          <input
            placeholder="Teléfono del cliente"
            aria-label="Teléfono del cliente"
            value={values.telefonoCliente}
            onChange={(e) => set('telefonoCliente', e.target.value)}
            className={inputClasses}
            required
          />
          {err('telefonoCliente') && <p className="mt-1 text-xs text-red-600">{err('telefonoCliente')}</p>}
        </div>

        <div>
          <input
            placeholder="Nombre del proyecto"
            aria-label="Nombre del proyecto"
            value={values.nombreProyecto}
            onChange={(e) => set('nombreProyecto', e.target.value)}
            className={inputClasses}
            required
          />
          {err('nombreProyecto') && <p className="mt-1 text-xs text-red-600">{err('nombreProyecto')}</p>}
        </div>

        <div className="col-span-full">
          <textarea
            placeholder="Descripción (opcional)"
            aria-label="Descripción (opcional)"
            value={values.descripcion}
            onChange={(e) => set('descripcion', e.target.value)}
            className={inputClasses}
            rows={2}
          />
        </div>

        <button
          type="submit"
          disabled={creando}
          className="col-span-full mt-1 rounded-md bg-emerald-600 px-4 py-2 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
        >
          {creando ? 'Creando…' : 'Crear proyecto'}
        </button>
      </form>
    </div>
  );
}
