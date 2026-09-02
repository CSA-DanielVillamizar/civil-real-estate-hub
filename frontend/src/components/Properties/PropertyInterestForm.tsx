import { useState, type FormEvent } from 'react';
import { useCreateLead } from '../../hooks/useCreateLead';
import { OrigenLead } from '../../types/common';
import { FormField, inputClasses } from '../common/FormField';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const TELEFONO_REGEX = /^[0-9]{7,15}$/;

// Formulario mínimo, independiente de LeadCaptureForm (que está acoplado al
// flujo de la calculadora de obra) — este solo necesita nombre/email/
// teléfono + la propiedad de interés, sin datosCalculoObra.
export function PropertyInterestForm({ propiedadId }: { propiedadId: string }) {
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [telefono, setTelefono] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});
  const { isSubmitting, error, fieldErrors, lead, enviar } = useCreateLead();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();

    const validationErrors: Record<string, string> = {};
    if (!nombre.trim()) validationErrors.nombre = 'Ingresa tu nombre.';
    if (!EMAIL_REGEX.test(email.trim())) validationErrors.email = 'Ingresa un correo válido.';
    if (!TELEFONO_REGEX.test(telefono.trim())) validationErrors.telefono = 'Teléfono de 7 a 15 dígitos.';
    setErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    enviar({
      nombre: nombre.trim(),
      email: email.trim(),
      telefono: telefono.trim(),
      origen: OrigenLead.FormularioContacto,
      propiedadDeInteresId: propiedadId,
    });
  }

  if (lead) {
    return (
      <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
        ¡Gracias! Un asesor te contactará pronto sobre esta propiedad.
      </div>
    );
  }

  const showError = (field: string) => fieldErrors[field]?.[0] ?? errors[field];

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-3">
      <h3 className="font-semibold text-slate-900">¿Te interesa esta propiedad?</h3>
      {error && <p className="text-sm text-red-600">{error}</p>}

      <FormField label="Nombre" htmlFor="interes-nombre" error={showError('nombre')}>
        <input
          id="interes-nombre"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
          className={inputClasses(Boolean(showError('nombre')))}
        />
      </FormField>

      <FormField label="Correo electrónico" htmlFor="interes-email" error={showError('email')}>
        <input
          id="interes-email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className={inputClasses(Boolean(showError('email')))}
        />
      </FormField>

      <FormField label="Teléfono" htmlFor="interes-telefono" error={showError('telefono')}>
        <input
          id="interes-telefono"
          type="tel"
          value={telefono}
          onChange={(e) => setTelefono(e.target.value)}
          className={inputClasses(Boolean(showError('telefono')))}
        />
      </FormField>

      <button
        type="submit"
        disabled={isSubmitting}
        className="mt-1 rounded-lg bg-emerald-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-emerald-700 disabled:opacity-50"
      >
        {isSubmitting ? 'Enviando…' : 'Quiero que me contacten'}
      </button>
    </form>
  );
}
