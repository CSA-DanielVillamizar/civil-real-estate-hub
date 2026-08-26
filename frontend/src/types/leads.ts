import type { DatosCalculoObra, EstimacionCosto, OrigenLead } from './common';

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
