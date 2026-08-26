import { useState } from 'react';
import { useBudgetCalculator } from '../../hooks/useBudgetCalculator';
import { useCreateLead } from '../../hooks/useCreateLead';
import { OrigenLead, type DatosCalculoObra } from '../../types/common';
import { CalculatorForm } from './CalculatorForm';
import { EstimateSummary } from './EstimateSummary';
import { LeadCaptureForm } from './LeadCaptureForm';
import type { LeadFormValues } from './validation';

type Step = 'form' | 'estimate' | 'success';

export function BudgetCalculator() {
  const [step, setStep] = useState<Step>('form');
  const [datosCalculoObra, setDatosCalculoObra] = useState<DatosCalculoObra | null>(null);

  const budgetCalculator = useBudgetCalculator();
  const createLead = useCreateLead();

  async function handleCalcular(datos: DatosCalculoObra) {
    setDatosCalculoObra(datos);
    const resultado = await budgetCalculator.calcular(datos);
    if (resultado) setStep('estimate');
  }

  async function handleEnviarLead(valores: LeadFormValues) {
    if (!datosCalculoObra) return;

    const resultado = await createLead.enviar({
      nombre: valores.nombre,
      email: valores.email,
      telefono: valores.telefono,
      origen: OrigenLead.CalculadoraObra,
      datosCalculoObra,
    });

    if (resultado) setStep('success');
  }

  function handleReiniciar() {
    setStep('form');
    setDatosCalculoObra(null);
    budgetCalculator.reset();
    createLead.reset();
  }

  return (
    <section className="mx-auto w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-lg sm:p-8">
      <header className="mb-6">
        <p className="text-sm font-semibold uppercase tracking-wide text-emerald-600">Calculadora de obra</p>
        <h2 className="mt-1 text-2xl font-bold text-slate-900">¿Cuánto cuesta construir tu proyecto?</h2>
        <p className="mt-2 text-sm text-slate-500">
          Obtén un estimado preliminar en segundos y recibe una cotización detallada de nuestros asesores.
        </p>
      </header>

      {budgetCalculator.error && step === 'form' && (
        <div role="alert" className="mb-5 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {budgetCalculator.error}
        </div>
      )}

      {createLead.error && step === 'estimate' && (
        <div role="alert" className="mb-5 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {createLead.error}
        </div>
      )}

      {step === 'form' && (
        <CalculatorForm isSubmitting={budgetCalculator.isCalculating} onSubmit={handleCalcular} />
      )}

      {step === 'estimate' && budgetCalculator.estimacion && (
        <div className="flex flex-col gap-6">
          <EstimateSummary estimacion={budgetCalculator.estimacion} />
          <LeadCaptureForm
            isSubmitting={createLead.isSubmitting}
            serverFieldErrors={createLead.fieldErrors}
            onSubmit={handleEnviarLead}
          />
          <button
            type="button"
            onClick={handleReiniciar}
            className="text-sm font-medium text-slate-500 underline-offset-2 hover:text-slate-700 hover:underline"
          >
            Calcular otro proyecto
          </button>
        </div>
      )}

      {step === 'success' && (
        <div className="flex flex-col items-center gap-3 py-6 text-center">
          <div className="flex h-14 w-14 items-center justify-center rounded-full bg-emerald-100 text-2xl text-emerald-600">
            ✓
          </div>
          <h3 className="text-xl font-bold text-slate-900">¡Listo! Ya recibimos tu solicitud</h3>
          <p className="max-w-sm text-sm text-slate-500">
            Un asesor de nuestro equipo te contactará pronto con una cotización detallada para tu proyecto.
          </p>
          <button
            type="button"
            onClick={handleReiniciar}
            className="mt-3 inline-flex items-center justify-center rounded-lg border border-slate-300 px-5 py-2.5 font-medium text-slate-700 transition hover:bg-slate-50"
          >
            Calcular otro proyecto
          </button>
        </div>
      )}
    </section>
  );
}
