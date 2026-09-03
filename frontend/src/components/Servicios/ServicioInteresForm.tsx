import { useState, type FormEvent } from 'react';
import { useCreateLead } from '../../hooks/useCreateLead';
import { OrigenLead, type ServicioDeInteres } from '../../types/common';
import { FormField, inputClasses } from '../common/FormField';
import { ConsentCheckbox } from '../common/ConsentCheckbox';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const TELEFONO_REGEX = /^[0-9]{7,15}$/;

interface ServicioInteresFormProps {
  servicio: ServicioDeInteres;
}

// Formulario de captura compartido por las secciones de Consultoría/Diseño
// Estructural e Interventoría/Presupuestos — las 2 líneas de negocio sin
// presencia digital (docs/02-business-case.md §3.2/§3.3). Mismo patrón que
// PropertyInterestForm: independiente del flujo de la calculadora de obra,
// solo agrega servicioDeInteres (fijo, según la sección) y un mensaje libre.
export function ServicioInteresForm({ servicio }: ServicioInteresFormProps) {
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [telefono, setTelefono] = useState('');
  const [mensaje, setMensaje] = useState('');
  const [aceptaPrivacidad, setAceptaPrivacidad] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const { isSubmitting, error, fieldErrors, lead, enviar } = useCreateLead();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();

    const validationErrors: Record<string, string> = {};
    if (!nombre.trim()) validationErrors.nombre = 'Ingresa tu nombre.';
    if (!EMAIL_REGEX.test(email.trim())) validationErrors.email = 'Ingresa un correo válido.';
    if (!TELEFONO_REGEX.test(telefono.trim())) validationErrors.telefono = 'Teléfono de 7 a 15 dígitos.';
    if (!aceptaPrivacidad) validationErrors.aceptaPrivacidad = 'Debes aceptar la política de privacidad para continuar.';
    setErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    enviar({
      nombre: nombre.trim(),
      email: email.trim(),
      telefono: telefono.trim(),
      origen: OrigenLead.FormularioContacto,
      servicioDeInteres: servicio,
      mensaje: mensaje.trim() || undefined,
    });
  }

  if (lead) {
    return (
      <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
        ¡Gracias! Un asesor técnico te contactará pronto.
      </div>
    );
  }

  const showError = (field: string) => fieldErrors[field]?.[0] ?? errors[field];

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-3">
      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
        <FormField label="Nombre" htmlFor={`${servicio}-nombre`} error={showError('nombre')}>
          <input
            id={`${servicio}-nombre`}
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
            className={inputClasses(Boolean(showError('nombre')))}
          />
        </FormField>

        <FormField label="Email" htmlFor={`${servicio}-email`} error={showError('email')}>
          <input
            id={`${servicio}-email`}
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className={inputClasses(Boolean(showError('email')))}
          />
        </FormField>
      </div>

      <FormField label="Teléfono" htmlFor={`${servicio}-telefono`} error={showError('telefono')}>
        <input
          id={`${servicio}-telefono`}
          value={telefono}
          onChange={(e) => setTelefono(e.target.value)}
          className={inputClasses(Boolean(showError('telefono')))}
        />
      </FormField>

      <FormField label="Cuéntanos tu proyecto (opcional)" htmlFor={`${servicio}-mensaje`}>
        <textarea
          id={`${servicio}-mensaje`}
          value={mensaje}
          onChange={(e) => setMensaje(e.target.value)}
          rows={3}
          className={inputClasses(false)}
        />
      </FormField>

      <ConsentCheckbox
        id={`${servicio}-acepta-privacidad`}
        checked={aceptaPrivacidad}
        onChange={setAceptaPrivacidad}
        error={showError('aceptaPrivacidad')}
      />

      <button
        type="submit"
        disabled={isSubmitting}
        className="self-start rounded-lg bg-emerald-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-emerald-700 disabled:opacity-50"
      >
        {isSubmitting ? 'Enviando…' : 'Solicitar contacto'}
      </button>
    </form>
  );
}
