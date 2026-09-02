export const EstadoProyecto = {
  Planificacion: 'Planificacion',
  EnEjecucion: 'EnEjecucion',
  Pausado: 'Pausado',
  Finalizado: 'Finalizado',
} as const;
export type EstadoProyecto = (typeof EstadoProyecto)[keyof typeof EstadoProyecto];

export const EstadoHito = {
  Pendiente: 'Pendiente',
  EnProgreso: 'EnProgreso',
  Completado: 'Completado',
} as const;
export type EstadoHito = (typeof EstadoHito)[keyof typeof EstadoHito];

export interface CrearProyectoObraRequest {
  nombreCliente: string;
  emailCliente: string;
  telefonoCliente: string;
  indicativoCliente?: string;
  nombreProyecto: string;
  descripcion?: string;
  propiedadId?: string;
}

export interface CrearProyectoObraResponse {
  id: string;
  tokenAcceso: string;
}

export interface AgregarHitoRequest {
  nombre: string;
  descripcion?: string;
  fechaEstimada?: string;
}

export interface HitoResponse {
  hitoId: string;
  nombre: string;
  estado: string;
}

export interface EstadoHitoResponse {
  hitoId: string;
  estado: string;
}

export interface EstadoProyectoResponse {
  id: string;
  estado: string;
}

export interface AgregarEvidenciaHitoResponse {
  hitoId: string;
  fotoEvidenciaUrl: string;
}

export interface Hito {
  id: string;
  nombre: string;
  descripcion?: string;
  orden: number;
  estado: EstadoHito;
  fechaEstimada?: string;
  fechaCompletado?: string;
  fotoEvidenciaUrl?: string;
}

export interface ProyectoObraDetalle {
  id: string;
  nombreCliente: string;
  emailCliente: string;
  telefonoCliente: string;
  nombreProyecto: string;
  descripcion?: string;
  propiedadId?: string;
  estado: EstadoProyecto;
  creadoEn: string;
  hitos: Hito[];
}

export interface ProyectoObraListItem {
  id: string;
  nombreCliente: string;
  nombreProyecto: string;
  estado: EstadoProyecto;
  creadoEn: string;
  totalHitos: number;
  hitosCompletados: number;
  tokenAcceso: string;
}
