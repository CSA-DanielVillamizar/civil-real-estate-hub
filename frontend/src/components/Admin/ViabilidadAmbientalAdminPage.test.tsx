import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ViabilidadAmbientalAdminPage } from './ViabilidadAmbientalAdminPage';
import * as viabilidadAmbientalService from '../../services/viabilidadAmbientalService';
import * as authService from '../../services/authService';
import { ApiError } from '../../types/api';
import type { SolicitudViabilidadAmbientalListItem } from '../../types/viabilidadAmbiental';
import type { LoginResponse } from '../../types/auth';

vi.mock('../../services/viabilidadAmbientalService', async (importOriginal) => ({
  ...(await importOriginal<typeof viabilidadAmbientalService>()),
  listarSolicitudesViabilidadAmbiental: vi.fn(),
  confirmarPagoViabilidadAmbiental: vi.fn(),
}));

vi.mock('../../services/authService', async (importOriginal) => ({
  ...(await importOriginal<typeof authService>()),
  login: vi.fn(),
}));

const SOLICITUD_MOCK: SolicitudViabilidadAmbientalListItem = {
  id: 'sol-123',
  nombre: 'Ana Restrepo',
  email: 'ana@example.com',
  telefono: '+573109876543',
  municipio: 'Rionegro',
  departamento: 'Antioquia',
  monto: 200_000,
  moneda: 'COP',
  estado: 'Solicitada',
  solicitadaEn: '2026-09-01T12:00:00Z',
};

const SESION_ADMIN: LoginResponse = {
  token: 'token-jwt-admin',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Daniel Villamizar',
  rol: 'Admin',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('ViabilidadAmbientalAdminPage', () => {
  it('pide email y contraseña antes de mostrar cualquier dato', () => {
    render(<ViabilidadAmbientalAdminPage />);

    expect(screen.getByPlaceholderText(/email/i)).toBeInTheDocument();
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).not.toHaveBeenCalled();
  });

  it('tras iniciar sesión como Admin, carga y muestra la lista de solicitudes', async () => {
    vi.mocked(authService.login).mockResolvedValue(SESION_ADMIN);
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockResolvedValue([SOLICITUD_MOCK]);
    const user = userEvent.setup();
    render(<ViabilidadAmbientalAdminPage />);

    await user.type(screen.getByPlaceholderText(/email/i), 'daniel@example.com');
    await user.type(screen.getByPlaceholderText(/contraseña/i), 'clave-correcta');
    await user.click(screen.getByRole('button', { name: /entrar/i }));

    expect(await screen.findByText('Ana Restrepo')).toBeInTheDocument();
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).toHaveBeenCalledWith(
      'token-jwt-admin',
      undefined,
      expect.anything(),
    );
  });

  it('un AsesorComercial autenticado no ve este panel', async () => {
    vi.mocked(authService.login).mockResolvedValue({ ...SESION_ADMIN, rol: 'AsesorComercial' });
    const user = userEvent.setup();
    render(<ViabilidadAmbientalAdminPage />);

    await user.type(screen.getByPlaceholderText(/email/i), 'asesor@example.com');
    await user.type(screen.getByPlaceholderText(/contraseña/i), 'clave-correcta');
    await user.click(screen.getByRole('button', { name: /entrar/i }));

    expect(await screen.findByText(/no tienes acceso/i)).toBeInTheDocument();
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).not.toHaveBeenCalled();
  });

  it('confirmar pago llama al servicio con el id y el token, y recarga la lista', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockResolvedValue([SOLICITUD_MOCK]);
    vi.mocked(viabilidadAmbientalService.confirmarPagoViabilidadAmbiental).mockResolvedValue({
      id: 'sol-123',
      estado: 'Pagada',
      pagoConfirmadoEn: '2026-09-01T12:05:00Z',
    });
    const user = userEvent.setup();
    render(<ViabilidadAmbientalAdminPage />);

    await screen.findByText('Ana Restrepo');
    await user.click(screen.getByRole('button', { name: /confirmar pago/i }));

    expect(viabilidadAmbientalService.confirmarPagoViabilidadAmbiental).toHaveBeenCalledWith('sol-123', 'token-jwt-admin');
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).toHaveBeenCalledTimes(2);
  });

  it('con un 401 al cargar, limpia la sesión guardada y vuelve a pedir login', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockRejectedValue(new ApiError(401));
    render(<ViabilidadAmbientalAdminPage />);

    expect(await screen.findByPlaceholderText(/email/i)).toBeInTheDocument();
    expect(localStorage.getItem('auth')).toBeNull();
  });
});
