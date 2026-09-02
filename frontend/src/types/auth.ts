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
