import { useState } from 'react';
import { useBudgetCalculator } from '../../hooks/useBudgetCalculator';
import { useCreateLead } from '../../hooks/useCreateLead';
import { useGenerarPresupuestoPdf } from '../../hooks/useGenerarPresupuestoPdf';
import { OrigenLead, type DatosCalculoObra } from '../../types/common';
import { CheckCircleIcon } from '../common/icons';
import { CalculatorForm } from './CalculatorForm';
import { EstimateSummary } from './EstimateSummary';
import { LeadCaptureForm } from './LeadCaptureForm';
import type { LeadFormValues } from './validation';

type Step = 'form' | 'estimate' | 'success';
type ViaExito = 'formulario' | 'pdf' | null;

export function BudgetCalculator() {
  const [step, setStep] = useState<Step>('form');
  const [datosCalculoObra, setDatosCalculoObra] = useState<DatosCalculoObra | null>(null);
  const [viaExito, setViaExito] = useState<ViaExito>(null);

  const budgetCalculator = useBudgetCalculator();
  const createLead = useCreateLead();
  const generarPdf = useGenerarPresupuestoPdf();

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

    if (resultado) {
      setViaExito('formulario');
      setStep('success');
    }
  }

  // Descargar el PDF también registra el lead (ya calificado, ver
  // GenerarPresupuestoPdfCommandHandler) — es una acción de conversión
  // alternativa a "dejar mis datos", no un paso adicional después.
  async function handleDescargarPdf(valores: LeadFormValues) {
    if (!datosCalculoObra) return;

    const exito = await generarPdf.generar({
      nombre: valores.nombre,
      email: valores.email,
      telefono: valores.telefono,
      origen: OrigenLead.CalculadoraObra,
      datosCalculoObra,
    });

    if (exito) {
      setViaExito('pdf');
      setStep('success');
    }
  }

  function handleReiniciar() {
    setStep('form');
    setDatosCalculoObra(null);
    setViaExito(null);
    budgetCalculator.reset();
    createLead.reset();
    generarPdf.reset();
  }

  const errorDelPasoEstimado = createLead.error ?? generarPdf.error;

  return (
    <section className="mx-auto w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-lg sm:p-8">
      <header className="mb-6">
        <p className="font-heading text-xs font-bold uppercase tracking-widest text-sky-600">Calculadora de obra</p>
        <h2 className="font-heading mt-1 text-2xl font-bold tracking-tight text-slate-900">
          ¿Cuánto cuesta construir tu proyecto?
        </h2>
        <p className="mt-2 text-sm text-slate-500">
          Obtén un estimado preliminar en segundos y recibe una cotización detallada de nuestros asesores.
        </p>
      </header>

      {step !== 'success' && <PasosIndicador pasoActual={step === 'form' ? 1 : 2} />}

      {budgetCalculator.error && step === 'form' && (
        <div role="alert" className="mb-5 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {budgetCalculator.error}
        </div>
      )}

      {errorDelPasoEstimado && step === 'estimate' && (
        <div role="alert" className="mb-5 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {errorDelPasoEstimado}
        </div>
      )}

      {step === 'form' && (
        <CalculatorForm isSubmitting={budgetCalculator.isCalculating} onSubmit={handleCalcular} />
      )}

      {step === 'estimate' && budgetCalculator.estimacion && (
        <div className="flex flex-col gap-6">
          <EstimateSummary estimacion={budgetCalculator.estimacion} areaConstruccionM2={datosCalculoObra?.areaConstruccionM2} />
          <LeadCaptureForm
            isSubmitting={createLead.isSubmitting}
            isGenerandoPdf={generarPdf.isGenerando}
            serverFieldErrors={{ ...generarPdf.fieldErrors, ...createLead.fieldErrors }}
            onSubmit={handleEnviarLead}
            onDescargarPdf={handleDescargarPdf}
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
          <div className="flex h-14 w-14 items-center justify-center rounded-full bg-emerald-100 text-emerald-600">
            <CheckCircleIcon className="h-7 w-7" />
          </div>
          <h3 className="font-heading text-xl font-bold text-slate-900">
            {viaExito === 'pdf' ? '¡Listo! Tu PDF se está descargando' : '¡Listo! Ya recibimos tu solicitud'}
          </h3>
          <p className="max-w-sm text-sm text-slate-500">
            {viaExito === 'pdf'
              ? 'Revisa tu carpeta de descargas. Un asesor de nuestro equipo también te contactará pronto con una cotización detallada.'
              : 'Un asesor de nuestro equipo te contactará pronto con una cotización detallada para tu proyecto.'}
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

// Indicador de progreso de 2 pasos reales (parámetros → resultado y
// contacto) — el mockup mostraba un wizard de 3 pasos con "Cómputo Métrico
// y Descarga PDF" como paso aparte, pero para nosotros la descarga del PDF
// es una de las dos acciones del mismo paso 2, no un tercer paso real.
function PasosIndicador({ pasoActual }: { pasoActual: 1 | 2 }) {
  const pasos: { numero: 1 | 2; titulo: string }[] = [
    { numero: 1, titulo: 'Parámetros del proyecto' },
    { numero: 2, titulo: 'Resultado y contacto' },
  ];

  return (
    <div className="mb-6 grid grid-cols-2 gap-2">
      {pasos.map((paso) => {
        const completado = paso.numero < pasoActual;
        const activo = paso.numero === pasoActual;
        return (
          <div
            key={paso.numero}
            className={`flex items-center gap-2 rounded-lg border px-3 py-2 ${
              activo ? 'border-slate-900 bg-slate-900' : completado ? 'border-emerald-200 bg-emerald-50' : 'border-slate-200 bg-slate-50'
            }`}
          >
            {completado ? (
              <CheckCircleIcon className="h-4 w-4 shrink-0 text-emerald-600" />
            ) : (
              <span
                className={`flex h-4 w-4 shrink-0 items-center justify-center rounded-full font-heading text-[10px] font-bold ${
                  activo ? 'bg-white text-slate-900' : 'bg-slate-300 text-white'
                }`}
              >
                {paso.numero}
              </span>
            )}
            <div className="flex flex-col leading-tight">
              <span className={`font-heading text-[10px] font-bold uppercase tracking-wider ${activo ? 'text-slate-300' : 'text-slate-400'}`}>
                Paso {paso.numero}
              </span>
              <span className={`font-heading text-xs font-semibold ${activo ? 'text-white' : completado ? 'text-emerald-800' : 'text-slate-500'}`}>
                {paso.titulo}
              </span>
            </div>
          </div>
        );
      })}
    </div>
  );
}
