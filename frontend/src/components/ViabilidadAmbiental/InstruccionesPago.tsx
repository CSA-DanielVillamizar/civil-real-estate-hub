import type { SolicitarViabilidadAmbientalResponse } from '../../types/viabilidadAmbiental';

interface InstruccionesPagoProps {
  resultado: SolicitarViabilidadAmbientalResponse;
  onNuevaSolicitud: () => void;
}

// datosBancarios puede llegar vacío (ver ViabilidadAmbientalOptions —
// todavía sin configurar en Azure): se muestra un aviso en vez de campos en
// blanco, sin romper el flujo.
export function InstruccionesPago({ resultado, onNuevaSolicitud }: InstruccionesPagoProps) {
  const { datosBancarios } = resultado;
  const hayDatosBancarios = Boolean(datosBancarios.numeroCuenta);

  return (
    <div className="flex flex-col gap-5">
      <div>
        <h3 className="text-lg font-bold text-slate-900">Solicitud registrada</h3>
        <p className="mt-1 text-sm text-slate-600">
          Recibirás también estas instrucciones por correo. Un consultor se pondrá en contacto tras confirmar el pago.
        </p>
      </div>

      <div className="rounded-lg border border-slate-200 bg-slate-50 p-5">
        <p className="text-sm text-slate-500">Valor del estudio</p>
        <p className="text-2xl font-bold text-slate-900">
          {resultado.monto.toLocaleString('es-CO')} {resultado.moneda}
        </p>
      </div>

      {hayDatosBancarios ? (
        <div className="rounded-lg border border-slate-200 p-5">
          <p className="mb-3 text-sm font-semibold text-slate-700">Transfiere a:</p>
          <dl className="grid grid-cols-2 gap-y-2 text-sm">
            <dt className="text-slate-500">Banco</dt>
            <dd className="text-slate-900">{datosBancarios.banco}</dd>
            <dt className="text-slate-500">Tipo de cuenta</dt>
            <dd className="text-slate-900">{datosBancarios.tipoCuenta}</dd>
            <dt className="text-slate-500">Número de cuenta</dt>
            <dd className="text-slate-900">{datosBancarios.numeroCuenta}</dd>
            <dt className="text-slate-500">Titular</dt>
            <dd className="text-slate-900">{datosBancarios.titularCuenta}</dd>
          </dl>
          {datosBancarios.qrImageUrl && (
            <img
              src={datosBancarios.qrImageUrl}
              alt="Código QR para transferencia"
              className="mt-4 h-40 w-40 rounded-lg border border-slate-200 object-contain"
            />
          )}
        </div>
      ) : (
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm text-amber-800">
          Estamos terminando de publicar los datos de la cuenta bancaria. Un asesor te los enviará directamente en las
          próximas horas.
        </div>
      )}

      <button
        type="button"
        onClick={onNuevaSolicitud}
        className="self-start text-sm font-medium text-emerald-700 hover:underline"
      >
        Registrar otra solicitud
      </button>
    </div>
  );
}
