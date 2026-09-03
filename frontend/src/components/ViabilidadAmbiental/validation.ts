// Validaciones en cliente — reflejan SolicitarViabilidadAmbientalRequestValidator
// (backend). Son una capa de UX, no la fuente de verdad: el backend vuelve a
// validar. Sin selector de propiedad ya catalogada (propiedadId): el
// frontend no tiene todavía una UI de navegación de propiedades — siempre
// se envía el camino "lote aún no catalogado" (departamento/municipio).

export interface ViabilidadAmbientalFormValues {
  nombre: string;
  email: string;
  telefono: string;
  departamento: string;
  municipio: string;
  direccionReferencia: string;
  aceptaPrivacidad: boolean;
}

export const initialViabilidadAmbientalValues: ViabilidadAmbientalFormValues = {
  nombre: '',
  email: '',
  telefono: '',
  departamento: '',
  municipio: '',
  direccionReferencia: '',
  aceptaPrivacidad: false,
};

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const TELEFONO_REGEX = /^[0-9]{7,15}$/;

export function validateViabilidadAmbientalForm(values: ViabilidadAmbientalFormValues): Record<string, string> {
  const errors: Record<string, string> = {};

  if (!values.nombre.trim()) {
    errors.nombre = 'Ingresa tu nombre.';
  } else if (values.nombre.trim().length > 150) {
    errors.nombre = 'El nombre no puede superar 150 caracteres.';
  }

  if (!values.email.trim()) {
    errors.email = 'Ingresa tu correo electrónico.';
  } else if (!EMAIL_REGEX.test(values.email.trim())) {
    errors.email = 'Ingresa un correo electrónico válido.';
  }

  if (!values.telefono.trim()) {
    errors.telefono = 'Ingresa tu teléfono.';
  } else if (!TELEFONO_REGEX.test(values.telefono.trim())) {
    errors.telefono = 'El teléfono debe tener entre 7 y 15 dígitos, sin espacios ni símbolos.';
  }

  if (!values.departamento.trim()) {
    errors.departamento = 'Ingresa el departamento.';
  } else if (values.departamento.trim().length > 100) {
    errors.departamento = 'El departamento no puede superar 100 caracteres.';
  }

  if (!values.municipio.trim()) {
    errors.municipio = 'Ingresa el municipio.';
  } else if (values.municipio.trim().length > 100) {
    errors.municipio = 'El municipio no puede superar 100 caracteres.';
  }

  if (values.direccionReferencia.trim().length > 250) {
    errors.direccionReferencia = 'La referencia no puede superar 250 caracteres.';
  }

  if (!values.aceptaPrivacidad) {
    errors.aceptaPrivacidad = 'Debes aceptar la política de privacidad para continuar.';
  }

  return errors;
}
