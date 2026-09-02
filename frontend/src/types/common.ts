// Espejo exacto de api/openapi.yaml (components.schemas) — Fase 2.
// Cualquier cambio a los contratos de la API debe reflejarse aquí primero.

export const OrigenLead = {
  CalculadoraObra: 'CalculadoraObra',
  FormularioContacto: 'FormularioContacto',
  LandingPage: 'LandingPage',
  Referido: 'Referido',
} as const;
export type OrigenLead = (typeof OrigenLead)[keyof typeof OrigenLead];

export const EstadoLead = {
  Nuevo: 'Nuevo',
  Contactado: 'Contactado',
  Calificado: 'Calificado',
  Convertido: 'Convertido',
  Descartado: 'Descartado',
  ContactoPendientePorReasignacion: 'ContactoPendientePorReasignacion',
} as const;
export type EstadoLead = (typeof EstadoLead)[keyof typeof EstadoLead];

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

export const TipoSuelo = {
  Arcilloso: 'Arcilloso',
  Arenoso: 'Arenoso',
  Rocoso: 'Rocoso',
  Franco: 'Franco',
  Limoso: 'Limoso',
} as const;
export type TipoSuelo = (typeof TipoSuelo)[keyof typeof TipoSuelo];

export const Topografia = {
  Plana: 'Plana',
  Inclinada: 'Inclinada',
  Irregular: 'Irregular',
} as const;
export type Topografia = (typeof Topografia)[keyof typeof Topografia];

export const TipoFuenteRetiro = {
  Rio: 'Rio',
  Quebrada: 'Quebrada',
  Bosque: 'Bosque',
  ViaPrincipal: 'ViaPrincipal',
  LineaAltaTension: 'LineaAltaTension',
} as const;
export type TipoFuenteRetiro = (typeof TipoFuenteRetiro)[keyof typeof TipoFuenteRetiro];

export const TipoMultimedia = {
  Foto: 'Foto',
  Plano: 'Plano',
  Render: 'Render',
  Video: 'Video',
} as const;
export type TipoMultimedia = (typeof TipoMultimedia)[keyof typeof TipoMultimedia];

export const UnidadMedidaArea = {
  M2: 'M2',
  Hectarea: 'Hectarea',
} as const;
export type UnidadMedidaArea = (typeof UnidadMedidaArea)[keyof typeof UnidadMedidaArea];

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
