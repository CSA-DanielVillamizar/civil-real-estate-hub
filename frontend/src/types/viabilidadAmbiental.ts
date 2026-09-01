export interface DatosBancarios {
  banco: string;
  tipoCuenta: string;
  numeroCuenta: string;
  titularCuenta: string;
  qrImageUrl: string;
}

export interface SolicitarViabilidadAmbientalRequest {
  nombre: string;
  email: string;
  telefono: string;
  indicativo?: string;
  propiedadId?: string;
  departamento?: string;
  municipio?: string;
  direccionReferencia?: string;
}

export interface SolicitarViabilidadAmbientalResponse {
  id: string;
  estado: string;
  monto: number;
  moneda: string;
  datosBancarios: DatosBancarios;
}

export interface SolicitudViabilidadAmbientalListItem {
  id: string;
  nombre: string;
  email: string;
  telefono: string;
  propiedadId?: string;
  municipio?: string;
  departamento?: string;
  monto: number;
  moneda: string;
  estado: string;
  solicitadaEn: string;
  pagoConfirmadoEn?: string;
}

export interface ConfirmarPagoViabilidadAmbientalResponse {
  id: string;
  estado: string;
  pagoConfirmadoEn: string;
}
