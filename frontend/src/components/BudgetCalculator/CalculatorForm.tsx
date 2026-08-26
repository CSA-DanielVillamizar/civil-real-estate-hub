import { useState, type FormEvent } from 'react';
import { FormField, inputClasses } from '../common/FormField';
import { TipoAcabado, TipoProyecto } from '../../types/common';
import {
  initialCalculatorValues,
  toDatosCalculoObra,
  validateCalculatorForm,
  type CalculatorFormValues,
} from './validation';
import type { DatosCalculoObra } from '../../types/common';

const TIPO_ACABADO_LABELS: Record<TipoAcabado, string> = {
  [TipoAcabado.Basico]: 'Básico',
  [TipoAcabado.Medio]: 'Medio',
  [TipoAcabado.Alto]: 'Alto / Premium',
};

const TIPO_PROYECTO_LABELS: Record<TipoProyecto, string> = {
  [TipoProyecto.Vivienda]: 'Vivienda',
  [TipoProyecto.Comercial]: 'Comercial',
  [TipoProyecto.Industrial]: 'Industrial',
};

interface CalculatorFormProps {
  isSubmitting: boolean;
  onSubmit: (datos: DatosCalculoObra) => void;
}

export function CalculatorForm({ isSubmitting, onSubmit }: CalculatorFormProps) {
  const [values, setValues] = useState<CalculatorFormValues>(initialCalculatorValues);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  function handleChange<K extends keyof CalculatorFormValues>(field: K, value: CalculatorFormValues[K]) {
    setValues((prev) => ({ ...prev, [field]: value }));
  }

  function handleBlur(field: keyof CalculatorFormValues) {
    setTouched((prev) => ({ ...prev, [field]: true }));
    setErrors(validateCalculatorForm({ ...values }));
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const validationErrors = validateCalculatorForm(values);
    setErrors(validationErrors);
    setTouched({ areaConstruccionM2: true, tipoAcabado: true, municipio: true, tipoProyecto: true });

    if (Object.keys(validationErrors).length > 0) return;

    onSubmit(toDatosCalculoObra(values));
  }

  const showError = (field: keyof CalculatorFormValues) => (touched[field] ? errors[field] : undefined);

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-5">
      <FormField label="Área de construcción (m²)" htmlFor="areaConstruccionM2" error={showError('areaConstruccionM2')}>
        <input
          id="areaConstruccionM2"
          type="number"
          inputMode="decimal"
          min={0}
          step="0.01"
          placeholder="Ej. 120"
          value={values.areaConstruccionM2}
          onChange={(e) => handleChange('areaConstruccionM2', e.target.value)}
          onBlur={() => handleBlur('areaConstruccionM2')}
          className={inputClasses(Boolean(showError('areaConstruccionM2')))}
        />
      </FormField>

      <FormField label="Tipo de proyecto" htmlFor="tipoProyecto" error={showError('tipoProyecto')}>
        <select
          id="tipoProyecto"
          value={values.tipoProyecto}
          onChange={(e) => handleChange('tipoProyecto', e.target.value as CalculatorFormValues['tipoProyecto'])}
          onBlur={() => handleBlur('tipoProyecto')}
          className={inputClasses(Boolean(showError('tipoProyecto')))}
        >
          <option value="" disabled>
            Selecciona una opción
          </option>
          {Object.values(TipoProyecto).map((tipo) => (
            <option key={tipo} value={tipo}>
              {TIPO_PROYECTO_LABELS[tipo]}
            </option>
          ))}
        </select>
      </FormField>

      <FormField label="Nivel de acabado" htmlFor="tipoAcabado" error={showError('tipoAcabado')}>
        <select
          id="tipoAcabado"
          value={values.tipoAcabado}
          onChange={(e) => handleChange('tipoAcabado', e.target.value as CalculatorFormValues['tipoAcabado'])}
          onBlur={() => handleBlur('tipoAcabado')}
          className={inputClasses(Boolean(showError('tipoAcabado')))}
        >
          <option value="" disabled>
            Selecciona una opción
          </option>
          {Object.values(TipoAcabado).map((tipo) => (
            <option key={tipo} value={tipo}>
              {TIPO_ACABADO_LABELS[tipo]}
            </option>
          ))}
        </select>
      </FormField>

      <FormField label="Municipio" htmlFor="municipio" error={showError('municipio')}>
        <input
          id="municipio"
          type="text"
          placeholder="Ej. Gómez Plata"
          value={values.municipio}
          onChange={(e) => handleChange('municipio', e.target.value)}
          onBlur={() => handleBlur('municipio')}
          className={inputClasses(Boolean(showError('municipio')))}
        />
      </FormField>

      <button
        type="submit"
        disabled={isSubmitting}
        className="mt-2 inline-flex items-center justify-center rounded-lg bg-emerald-600 px-5 py-3 font-semibold text-white shadow-sm transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-emerald-300"
      >
        {isSubmitting ? 'Calculando…' : 'Calcular estimado'}
      </button>
    </form>
  );
}
