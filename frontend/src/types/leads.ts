import type { DatosCalculoObra, EstadoLead, EstimacionCosto, OrigenLead } from './common';

export interface CreateLeadRequest {
  nombre: string;
  email: string;
  telefono: string;
  indicativo?: string;
  origen: OrigenLead;
  propiedadDeInteresId?: string;
  datosCalculoObra?: DatosCalculoObra;
}

export interface CreateLeadResponse {
  id: string;
  estado: string;
  estimacionCosto?: EstimacionCosto;
}

export interface LeadListItem {
  id: string;
  nombre: string;
  email: string;
  telefono: string;
  origen: OrigenLead;
  estado: EstadoLead;
  capturadoEn: string;
  propiedadDeInteresId?: string;
  estimacionMontoMinimo?: number;
  estimacionMontoMaximo?: number;
  estimacionMoneda?: string;
}

export interface LeadEstadoResponse {
  id: string;
  estado: string;
}

export interface GetLeadsAdminParams {
  estado?: EstadoLead;
}
