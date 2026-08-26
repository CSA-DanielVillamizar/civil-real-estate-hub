import type { ReactNode } from 'react';

interface FormFieldProps {
  label: string;
  htmlFor: string;
  error?: string;
  hint?: string;
  children: ReactNode;
}

export function FormField({ label, htmlFor, error, hint, children }: FormFieldProps) {
  return (
    <div className="flex flex-col gap-1.5">
      <label htmlFor={htmlFor} className="text-sm font-medium text-slate-700">
        {label}
      </label>
      {children}
      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : hint ? (
        <p className="text-sm text-slate-400">{hint}</p>
      ) : null}
    </div>
  );
}

const baseInputClasses =
  'w-full rounded-lg border bg-white px-3.5 py-2.5 text-slate-900 shadow-sm outline-none transition focus:ring-2 focus:ring-emerald-500/40 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-400';

export function inputClasses(hasError: boolean): string {
  return `${baseInputClasses} ${hasError ? 'border-red-400 focus:border-red-500' : 'border-slate-300 focus:border-emerald-500'}`;
}
