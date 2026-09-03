interface ConsentCheckboxProps {
  id: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
  error?: string;
}

// Autorización explícita de tratamiento de datos personales (Ley 1581 de
// 2012 — Habeas Data, Colombia): un checkbox sin marcar por defecto, con
// link a la política, requerido antes de poder enviar cualquiera de los
// formularios públicos que recogen nombre/email/teléfono.
export function ConsentCheckbox({ id, checked, onChange, error }: ConsentCheckboxProps) {
  return (
    <div>
      <label htmlFor={id} className="flex items-start gap-2 text-sm text-slate-600">
        <input
          id={id}
          type="checkbox"
          checked={checked}
          onChange={(e) => onChange(e.target.checked)}
          className="mt-0.5 h-4 w-4 shrink-0 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500"
        />
        <span>
          He leído y acepto la{' '}
          <a
            href="/politica-de-privacidad"
            target="_blank"
            rel="noopener noreferrer"
            className="text-emerald-700 underline hover:text-emerald-800"
          >
            política de privacidad
          </a>{' '}
          y autorizo el tratamiento de mis datos personales.
        </span>
      </label>
      {error && <p className="mt-1 text-xs text-red-600">{error}</p>}
    </div>
  );
}
