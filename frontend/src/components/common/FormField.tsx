import { cloneElement, isValidElement, type ReactElement, type ReactNode } from 'react';

interface FormFieldProps {
  label: string;
  htmlFor: string;
  error?: string;
  hint?: string;
  children: ReactNode;
}

// Accesibilidad (gap #8): antes el error/hint solo se veía (el texto rojo
// bajo el campo) — un lector de pantalla que enfoca el input no se enteraba
// de nada. Se centraliza acá (en vez de tocar cada input/select/textarea de
// cada formulario) inyectando aria-invalid/aria-describedby vía
// cloneElement sobre el único hijo (siempre un elemento de formulario
// controlado) — arregla todos los campos que usan FormField de una vez.
export function FormField({ label, htmlFor, error, hint, children }: FormFieldProps) {
  const describedBy = error ? `${htmlFor}-error` : hint ? `${htmlFor}-hint` : undefined;

  const field = isValidElement(children)
    ? cloneElement(children as ReactElement<Record<string, unknown>>, {
        'aria-invalid': error ? true : undefined,
        'aria-describedby': describedBy,
      })
    : children;

  return (
    <div className="flex flex-col gap-1.5">
      <label htmlFor={htmlFor} className="text-sm font-medium text-slate-700">
        {label}
      </label>
      {field}
      {error ? (
        <p id={`${htmlFor}-error`} className="text-sm text-red-600">
          {error}
        </p>
      ) : hint ? (
        <p id={`${htmlFor}-hint`} className="text-sm text-slate-400">
          {hint}
        </p>
      ) : null}
    </div>
  );
}

const baseInputClasses =
  'w-full rounded-lg border bg-white px-3.5 py-2.5 text-slate-900 shadow-sm outline-none transition focus:ring-2 focus:ring-emerald-500/40 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-400';

export function inputClasses(hasError: boolean): string {
  return `${baseInputClasses} ${hasError ? 'border-red-400 focus:border-red-500' : 'border-slate-300 focus:border-emerald-500'}`;
}
