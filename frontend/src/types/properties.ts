import type { EstadoPropiedad, TipoFuenteRetiro, TipoInmueble, TipoMultimedia, TipoSuelo, Topografia, UnidadMedidaArea } from './common';

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

export interface GetPropertiesAdminParams {
  estado?: EstadoPropiedad;
  page?: number;
  pageSize?: number;
}

export interface RetiroAmbiental {
  tipoFuente: TipoFuenteRetiro;
  distanciaMinimaMetros: number;
  normativaAplicable: string;
}

export interface ArchivoMultimedia {
  id: string;
  url: string;
  tipo: TipoMultimedia;
  orden: number;
}

export interface PropertyDetailResponse {
  id: string;
  titulo: string;
  descripcion: string;
  tipoInmueble: TipoInmueble;
  precio: number;
  moneda: string;
  direccion: string;
  municipio: string;
  departamento: string;
  latitud?: number;
  longitud?: number;
  areaTerrenoM2: number;
  areaConstruidaM2?: number;
  pendientePorcentaje: number;
  tipoSuelo: TipoSuelo;
  topografia: Topografia;
  nivelFreaticoMetros?: number;
  estado: EstadoPropiedad;
  esViableConstructivamente: boolean;
  restriccionesViabilidad: string[];
  retirosAmbientales: RetiroAmbiental[];
  multimedia: ArchivoMultimedia[];
}

export interface CrearPropiedadRequest {
  titulo: string;
  descripcion: string;
  tipoInmueble: TipoInmueble;
  precio: number;
  moneda: string;
  direccion: string;
  municipio: string;
  departamento: string;
  latitud?: number;
  longitud?: number;
  areaTerrenoValor: number;
  areaTerrenoUnidad: UnidadMedidaArea;
  areaConstruidaValor?: number;
  areaConstruidaUnidad?: UnidadMedidaArea;
  pendientePorcentaje: number;
  tipoSuelo: TipoSuelo;
  topografia: Topografia;
  nivelFreaticoMetros?: number;
  retirosAmbientales?: RetiroAmbiental[];
}

export interface CrearPropiedadResponse {
  id: string;
  estado: string;
}

export interface AgregarMultimediaResponse {
  propiedadId: string;
  url: string;
  tipo: string;
}

export interface PublicarPropiedadResponse {
  id: string;
  estado: string;
}

export interface ActualizarDatosBasicosPropiedadRequest {
  titulo: string;
  descripcion: string;
  precio: number;
  moneda: string;
}

export interface ActualizarDatosBasicosPropiedadResponse {
  id: string;
  titulo: string;
  descripcion: string;
  precio: number;
  moneda: string;
}
