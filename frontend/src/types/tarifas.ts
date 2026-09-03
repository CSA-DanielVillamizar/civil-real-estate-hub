import type { ServicioDeInteres } from './common';

export interface PaqueteTarifa {
  id: string;
  servicioRelacionado: string;
  titulo: string;
  descripcion: string;
  precioDesde: number | null;
  precioHasta: number | null;
  unidadPrecio: string;
  moneda: string;
  publicado: boolean;
  creadoEn: string;
}

export interface CrearPaqueteTarifaRequest {
  servicioRelacionado: ServicioDeInteres;
  titulo: string;
  descripcion: string;
  precioDesde?: number;
  precioHasta?: number;
  unidadPrecio: string;
  moneda: string;
}

export interface ActualizarPaqueteTarifaRequest {
  titulo: string;
  descripcion: string;
  precioDesde?: number;
  precioHasta?: number;
  unidadPrecio: string;
  servicioRelacionado: ServicioDeInteres;
}
