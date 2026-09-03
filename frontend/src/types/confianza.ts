import type { ServicioDeInteres } from './common';

export const TipoContenidoConfianza = {
  Testimonio: 'Testimonio',
  Portafolio: 'Portafolio',
} as const;
export type TipoContenidoConfianza = (typeof TipoContenidoConfianza)[keyof typeof TipoContenidoConfianza];

export interface ContenidoConfianza {
  id: string;
  tipo: string;
  titulo: string;
  descripcion: string;
  municipio: string | null;
  servicioRelacionado: string;
  publicado: boolean;
  creadoEn: string;
}

export interface CrearContenidoConfianzaRequest {
  tipo: TipoContenidoConfianza;
  titulo: string;
  descripcion: string;
  municipio?: string;
  servicioRelacionado: ServicioDeInteres;
}

export interface ActualizarContenidoConfianzaRequest {
  titulo: string;
  descripcion: string;
  municipio?: string;
  servicioRelacionado: ServicioDeInteres;
}
