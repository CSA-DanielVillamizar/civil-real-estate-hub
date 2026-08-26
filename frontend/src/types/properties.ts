import type { EstadoPropiedad, TipoInmueble } from './common';

export interface PropertyResponse {
  id: string;
  titulo: string;
  tipoInmueble: TipoInmueble;
  precio: number;
  moneda: string;
  municipio: string;
  departamento: string;
  areaTerrenoM2: number;
  areaConstruidaM2?: number;
  estado: EstadoPropiedad;
  fotoPrincipalUrl?: string;
  esViableConstructivamente: boolean;
}

export interface PagedPropertyResponse {
  items: PropertyResponse[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface GetPropertiesParams {
  tipoInmueble?: TipoInmueble;
  municipio?: string;
  precioMin?: number;
  precioMax?: number;
  areaMin?: number;
  areaMax?: number;
  soloViablesConstructivamente?: boolean;
  page?: number;
  pageSize?: number;
}
