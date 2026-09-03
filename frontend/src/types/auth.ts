export const RolUsuario = {
  Admin: 'Admin',
  AsesorComercial: 'AsesorComercial',
} as const;
export type RolUsuario = (typeof RolUsuario)[keyof typeof RolUsuario];

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiraEn: string;
  nombre: string;
  rol: RolUsuario;
}

export interface CrearUsuarioRequest {
  nombre: string;
  email: string;
  password: string;
  rol: RolUsuario;
}

export interface CrearUsuarioResponse {
  id: string;
  nombre: string;
  email: string;
  rol: string;
}

export interface UsuarioListItem {
  id: string;
  nombre: string;
  email: string;
  rol: string;
  activo: boolean;
  creadoEn: string;
}
