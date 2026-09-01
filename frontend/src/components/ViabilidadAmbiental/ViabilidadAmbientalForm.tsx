import { useState, type FormEvent } from 'react';
import { FormField, inputClasses } from '../common/FormField';
import {
  initialViabilidadAmbientalValues,
  validateViabilidadAmbientalForm,
  type ViabilidadAmbientalFormValues,
} from './validation';
import type { SolicitarViabilidadAmbientalRequest } from '../../types/viabilidadAmbiental';

interface ViabilidadAmbientalFormProps {
  isSubmitting: boolean;
  serverFieldErrors: Record<string, string[]>;
  onSubmit: (request: SolicitarViabilidadAmbientalRequest) => void;
}

export function ViabilidadAmbientalForm({ isSubmitting, serverFieldErrors, onSubmit }: ViabilidadAmbientalFormProps) {
  const [values, setValues] = useState<ViabilidadAmbientalFormValues>(initialViabilidadAmbientalValues);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  function handleChange<K extends keyof ViabilidadAmbientalFormValues>(field: K, value: ViabilidadAmbientalFormValues[K]) {
    setValues((prev) => ({ ...prev, [field]: value }));
  }

  function handleBlur(field: keyof ViabilidadAmbientalFormValues) {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setErrors(validateViabilidadAmbientalForm({ ...values }));
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const validationErrors = validateViabilidadAmbientalForm(values);
    setErrors(validationErrors);
    setTouched({ nombre: true, email: true, telefono: true, departamento: true, municipio: true });

    if (Object.keys(validationErrors).length > 0) return;

    onSubmit({
      nombre: values.nombre.trim(),
      email: values.email.trim(),
      telefono: values.telefono.trim(),
      departamento: values.departamento.trim(),
      municipio: values.municipio.trim(),
      direccionReferencia: values.direccionReferencia.trim() || undefined,
    });
  }

  const showError = (field: keyof ViabilidadAmbientalFormValues) => {
    if (serverFieldErrors[field]?.length) return serverFieldErrors[field][0];
    return touched[field] ? errors[field] : undefined;
  };

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-5">
      <p className="text-sm text-slate-600">
        Un dictamen técnico preliminar sobre retiros ambientales y pendiente del terreno, antes de comprometerte con el
        lote. El pago se hace por transferencia — te mostramos los datos al enviar.
      </p>

      <FormField label="Nombre completo" htmlFor="va-nombre" error={showError('nombre')}>
        <input
          id="va-nombre"
          type="text"
          placeholder="Tu nombre"
          value={values.nombre}
          onChange={(e) => handleChange('nombre', e.target.value)}
          onBlur={() => handleBlur('nombre')}
          className={inputClasses(Boolean(showError('nombre')))}
        />
      </FormField>

      <FormField label="Correo electrónico" htmlFor="va-email" error={showError('email')}>
        <input
          id="va-email"
          type="email"
          placeholder="tucorreo@ejemplo.com"
          value={values.email}
          onChange={(e) => handleChange('email', e.target.value)}
          onBlur={() => handleBlur('email')}
          className={inputClasses(Boolean(showError('email')))}
        />
      </FormField>

      <FormField label="Teléfono" htmlFor="va-telefono" error={showError('telefono')} hint="Solo dígitos, sin indicativo (ej. 3001234567).">
        <input
          id="va-telefono"
          type="tel"
          placeholder="3001234567"
          value={values.telefono}
          onChange={(e) => handleChange('telefono', e.target.value)}
          onBlur={() => handleBlur('telefono')}
          className={inputClasses(Boolean(showError('telefono')))}
        />
      </FormField>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
        <FormField label="Departamento" htmlFor="va-departamento" error={showError('departamento')}>
          <input
            id="va-departamento"
            type="text"
            placeholder="Antioquia"
            value={values.departamento}
            onChange={(e) => handleChange('departamento', e.target.value)}
            onBlur={() => handleBlur('departamento')}
            className={inputClasses(Boolean(showError('departamento')))}
          />
        </FormField>

        <FormField label="Municipio" htmlFor="va-municipio" error={showError('municipio')}>
          <input
            id="va-municipio"
            type="text"
            placeholder="Rionegro"
            value={values.municipio}
            onChange={(e) => handleChange('municipio', e.target.value)}
            onBlur={() => handleBlur('municipio')}
            className={inputClasses(Boolean(showError('municipio')))}
          />
        </FormField>
      </div>

      <FormField
        label="Referencia del lote (opcional)"
        htmlFor="va-direccion"
        error={showError('direccionReferencia')}
        hint="Vereda, cerca a qué, coordenadas si las tienes."
      >
        <input
          id="va-direccion"
          type="text"
          placeholder="Vereda La Primavera, cerca al puente"
          value={values.direccionReferencia}
          onChange={(e) => handleChange('direccionReferencia', e.target.value)}
          onBlur={() => handleBlur('direccionReferencia')}
          className={inputClasses(Boolean(showError('direccionReferencia')))}
        />
      </FormField>

      <button
        type="submit"
        disabled={isSubmitting}
        className="mt-2 inline-flex items-center justify-center rounded-lg bg-emerald-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-emerald-300"
      >
        {isSubmitting ? 'Enviando…' : 'Solicitar estudio de viabilidad ambiental'}
      </button>
    </form>
  );
}
