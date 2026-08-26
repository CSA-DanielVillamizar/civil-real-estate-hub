import { TipoAcabado, TipoProyecto, type DatosCalculoObra } from '../../types/common';

// Validaciones en cliente — reflejan las reglas de FluentValidation del
// backend (DatosCalculoObraDtoValidator, CreateLeadRequestValidator, Fase 2).
// Son una capa de UX, no la fuente de verdad: el backend vuelve a validar.

export interface CalculatorFormValues {
  areaConstruccionM2: string;
  tipoAcabado: TipoAcabado | '';
  municipio: string;
  tipoProyecto: TipoProyecto | '';
}

export const initialCalculatorValues: CalculatorFormValues = {
  areaConstruccionM2: '',
  tipoAcabado: '',
  municipio: '',
  tipoProyecto: '',
};

export function validateCalculatorForm(values: CalculatorFormValues): Record<string, string> {
  const errors: Record<string, string> = {};

  const area = Number(values.areaConstruccionM2);
  if (!values.areaConstruccionM2.trim()) {
    errors.areaConstruccionM2 = 'Ingresa el área de construcción.';
  } else if (Number.isNaN(area) || area <= 0) {
    errors.areaConstruccionM2 = 'El área debe ser un número mayor que 0.';
  } else if (area > 100_000) {
    errors.areaConstruccionM2 = 'El área no puede superar 100.000 m².';
  }

  if (!values.tipoAcabado) errors.tipoAcabado = 'Selecciona el tipo de acabado.';
  if (!values.tipoProyecto) errors.tipoProyecto = 'Selecciona el tipo de proyecto.';

  if (!values.municipio.trim()) {
    errors.municipio = 'Ingresa el municipio.';
  } else if (values.municipio.trim().length > 100) {
    errors.municipio = 'El municipio no puede superar 100 caracteres.';
  }

  return errors;
}

export function toDatosCalculoObra(values: CalculatorFormValues): DatosCalculoObra {
  return {
    areaConstruccionM2: Number(values.areaConstruccionM2),
    tipoAcabado: values.tipoAcabado as TipoAcabado,
    municipio: values.municipio.trim(),
    tipoProyecto: values.tipoProyecto as TipoProyecto,
  };
}

export interface LeadFormValues {
  nombre: string;
  email: string;
  telefono: string;
}

export const initialLeadValues: LeadFormValues = { nombre: '', email: '', telefono: '' };

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const TELEFONO_REGEX = /^[0-9]{7,15}$/;

export function validateLeadForm(values: LeadFormValues): Record<string, string> {
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

  return errors;
}
