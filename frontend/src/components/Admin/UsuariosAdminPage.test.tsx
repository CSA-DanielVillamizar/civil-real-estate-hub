import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { UsuariosAdminPage } from './UsuariosAdminPage';
import * as authService from '../../services/authService';
import type { UsuarioListItem } from '../../types/auth';

vi.mock('../../services/authService', async (importOriginal) => ({
  ...(await importOriginal<typeof authService>()),
  getUsuarios: vi.fn(),
  crearUsuario: vi.fn(),
  cambiarActivoUsuario: vi.fn(),
}));

const SESION_ADMIN = {
  token: 'token-jwt-admin',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Daniel Villamizar',
  rol: 'Admin',
};

const USUARIO_MOCK: UsuarioListItem = {
  id: 'usr-1',
  nombre: 'Laura Gómez',
  email: 'laura@example.com',
  rol: 'AsesorComercial',
  activo: true,
  creadoEn: '2026-09-01T12:00:00Z',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('UsuariosAdminPage', () => {
  it('con sesión de Admin, carga y muestra los usuarios existentes', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(authService.getUsuarios).mockResolvedValue([USUARIO_MOCK]);

    render(<UsuariosAdminPage />);

    expect(await screen.findByText('Laura Gómez')).toBeInTheDocument();
    expect(screen.getByText(/activo/i)).toBeInTheDocument();
  });

  it('desactivar un usuario llama al servicio con activo=false', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(authService.getUsuarios).mockResolvedValue([USUARIO_MOCK]);
    vi.mocked(authService.cambiarActivoUsuario).mockResolvedValue({ id: 'usr-1', activo: false });
    const user = userEvent.setup();

    render(<UsuariosAdminPage />);
    await screen.findByText('Laura Gómez');
    await user.click(screen.getByRole('button', { name: /desactivar/i }));

    expect(authService.cambiarActivoUsuario).toHaveBeenCalledWith('usr-1', false, 'token-jwt-admin');
  });

  it('crear un usuario nuevo llama al servicio y muestra el aviso de confirmación', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(authService.getUsuarios).mockResolvedValue([]);
    vi.mocked(authService.crearUsuario).mockResolvedValue({ id: 'usr-2', nombre: 'Nuevo Asesor', email: 'nuevo@example.com', rol: 'AsesorComercial' });
    const user = userEvent.setup();

    render(<UsuariosAdminPage />);
    await screen.findByText(/aún no hay usuarios/i);

    await user.type(screen.getByPlaceholderText(/^nombre$/i), 'Nuevo Asesor');
    await user.type(screen.getByPlaceholderText(/^email$/i), 'nuevo@example.com');
    await user.type(screen.getByPlaceholderText(/contraseña temporal/i), 'claveSegura123');
    await user.click(screen.getByRole('button', { name: /crear usuario/i }));

    expect(authService.crearUsuario).toHaveBeenCalledWith(
      { nombre: 'Nuevo Asesor', email: 'nuevo@example.com', password: 'claveSegura123', rol: 'AsesorComercial' },
      'token-jwt-admin',
    );
    expect(await screen.findByText(/usuario creado/i)).toBeInTheDocument();
  });
});
