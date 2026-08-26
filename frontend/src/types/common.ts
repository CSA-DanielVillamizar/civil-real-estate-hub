// Espejo exacto de api/openapi.yaml (components.schemas) — Fase 2.
// Cualquier cambio a los contratos de la API debe reflejarse aquí primero.

export const OrigenLead = {
  CalculadoraObra: 'CalculadoraObra',
  FormularioContacto: 'FormularioContacto',
  LandingPage: 'LandingPage',
  Referido: 'Referido',
} as const;
export type OrigenLead = (typeof OrigenLead)[keyof typeof OrigenLead];

export const TipoAcabado = {
  Basico: 'Basico',
  Medio: 'Medio',
  Alto: 'Alto',
} as const;
export type TipoAcabado = (typeof TipoAcabado)[keyof typeof TipoAcabado];

export const TipoProyecto = {
  Vivienda: 'Vivienda',
  Comercial: 'Comercial',
  Industrial: 'Industrial',
} as const;
export type TipoProyecto = (typeof TipoProyecto)[keyof typeof TipoProyecto];

export const TipoInmueble = {
  Lote: 'Lote',
  Casa: 'Casa',
  Apartamento: 'Apartamento',
  Local: 'Local',
  Bodega: 'Bodega',
  Finca: 'Finca',
} as const;
export type TipoInmueble = (typeof TipoInmueble)[keyof typeof TipoInmueble];

export const EstadoPropiedad = {
  Borrador: 'Borrador',
  Publicada: 'Publicada',
  Reservada: 'Reservada',
  Vendida: 'Vendida',
  Arrendada: 'Arrendada',
  Retirada: 'Retirada',
} as const;
export type EstadoPropiedad = (typeof EstadoPropiedad)[keyof typeof EstadoPropiedad];

export interface DatosCalculoObra {
  areaConstruccionM2: number;
  tipoAcabado: TipoAcabado;
  municipio: string;
  tipoProyecto: TipoProyecto;
}

export interface DesgloseItem {
  categoria: string;
  monto: number;
}

export interface EstimacionCosto {
  montoMinimo: number;
  montoMaximo: number;
  moneda: string;
  desglose: DesgloseItem[];
}
