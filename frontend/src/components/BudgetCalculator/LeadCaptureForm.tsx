import { useState, type FormEvent } from 'react';
import { FormField, inputClasses } from '../common/FormField';
import { ConsentCheckbox } from '../common/ConsentCheckbox';
import { initialLeadValues, validateLeadForm, type LeadFormValues } from './validation';

interface LeadCaptureFormProps {
  isSubmitting: boolean;
  isGenerandoPdf: boolean;
  serverFieldErrors: Record<string, string[]>;
  onSubmit: (valores: LeadFormValues) => void;
  onDescargarPdf: (valores: LeadFormValues) => void;
}

export function LeadCaptureForm({
  isSubmitting,
  isGenerandoPdf,
  serverFieldErrors,
  onSubmit,
  onDescargarPdf,
}: LeadCaptureFormProps) {
  const [values, setValues] = useState<LeadFormValues>(initialLeadValues);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  function handleChange<K extends keyof LeadFormValues>(field: K, value: LeadFormValues[K]) {
    setValues((prev) => ({ ...prev, [field]: value }));
  }

  function handleBlur(field: keyof LeadFormValues) {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setErrors(validateLeadForm({ ...values }));
  }

  // Ambas acciones (dejar los datos vs. descargar el PDF) parten de los
  // mismos campos y la misma validación — solo difieren en qué caso de uso
  // disparan al final.
  function validarYEjecutar(accion: (valores: LeadFormValues) => void) {
    const validationErrors = validateLeadForm(values);
    setErrors(validationErrors);
    setTouched({ nombre: true, email: true, telefono: true, aceptaPrivacidad: true });

    if (Object.keys(validationErrors).length > 0) return;

    accion(values);
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    validarYEjecutar(onSubmit);
  }

  function handleDescargarPdf() {
    validarYEjecutar(onDescargarPdf);
  }

  const showError = (field: keyof LeadFormValues) => {
    if (serverFieldErrors[field]?.length) return serverFieldErrors[field][0];
    return touched[field] ? errors[field] : undefined;
  };

  const deshabilitado = isSubmitting || isGenerandoPdf;

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-5">
      <p className="text-sm text-slate-600">
        Déjanos tus datos y un asesor te contactará con una cotización detallada para tu proyecto.
      </p>

      <FormField label="Nombre completo" htmlFor="nombre" error={showError('nombre')}>
        <input
          id="nombre"
          type="text"
          placeholder="Tu nombre"
          value={values.nombre}
          onChange={(e) => handleChange('nombre', e.target.value)}
          onBlur={() => handleBlur('nombre')}
          className={inputClasses(Boolean(showError('nombre')))}
        />
      </FormField>

      <FormField label="Correo electrónico" htmlFor="email" error={showError('email')}>
        <input
          id="email"
          type="email"
          placeholder="tucorreo@ejemplo.com"
          value={values.email}
          onChange={(e) => handleChange('email', e.target.value)}
          onBlur={() => handleBlur('email')}
          className={inputClasses(Boolean(showError('email')))}
        />
      </FormField>

      <FormField label="Teléfono" htmlFor="telefono" error={showError('telefono')} hint="Solo dígitos, sin indicativo (ej. 3001234567).">
        <input
          id="telefono"
          type="tel"
          placeholder="3001234567"
          value={values.telefono}
          onChange={(e) => handleChange('telefono', e.target.value)}
          onBlur={() => handleBlur('telefono')}
          className={inputClasses(Boolean(showError('telefono')))}
        />
      </FormField>

      <ConsentCheckbox
        id="lead-acepta-privacidad"
        checked={values.aceptaPrivacidad}
        onChange={(checked) => handleChange('aceptaPrivacidad', checked)}
        error={showError('aceptaPrivacidad')}
      />

      <div className="mt-2 flex flex-col gap-3 sm:flex-row">
        <button
          type="button"
          onClick={handleDescargarPdf}
          disabled={deshabilitado}
          className="inline-flex flex-1 items-center justify-center gap-2 rounded-lg border-2 border-emerald-600 px-5 py-3 font-semibold text-emerald-700 shadow-sm transition hover:bg-emerald-50 disabled:cursor-not-allowed disabled:border-emerald-300 disabled:text-emerald-300"
        >
          {isGenerandoPdf ? 'Generando PDF…' : 'Descargar presupuesto en PDF'}
        </button>

        <button
          type="submit"
          disabled={deshabilitado}
          className="inline-flex flex-1 items-center justify-center rounded-lg bg-slate-900 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:bg-slate-400"
        >
          {isSubmitting ? 'Enviando…' : 'Quiero mi cotización detallada'}
        </button>
      </div>
    </form>
  );
}
