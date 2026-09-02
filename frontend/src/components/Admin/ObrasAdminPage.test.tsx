import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ObrasAdminPage } from './ObrasAdminPage';
import * as obrasService from '../../services/obrasService';
import type { ProyectoObraListItem } from '../../types/obras';

vi.mock('../../services/obrasService', async (importOriginal) => ({
  ...(await importOriginal<typeof obrasService>()),
  getProyectosObraAdmin: vi.fn(),
  crearProyectoObra: vi.fn(),
}));

const SESION_ADMIN = {
  token: 'token-jwt-admin',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Daniel Villamizar',
  rol: 'Admin',
};

const PROYECTO_MOCK: ProyectoObraListItem = {
  id: 'proy-1',
  nombreCliente: 'Ana Restrepo',
  nombreProyecto: 'Interventoría casa campestre',
  estado: 'EnEjecucion',
  creadoEn: '2026-09-01T12:00:00Z',
  totalHitos: 3,
  hitosCompletados: 1,
  tokenAcceso: 'abc123',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('ObrasAdminPage', () => {
  it('pide iniciar sesión antes de mostrar cualquier proyecto', () => {
    render(<ObrasAdminPage />);

    expect(screen.getByPlaceholderText(/email/i)).toBeInTheDocument();
    expect(obrasService.getProyectosObraAdmin).not.toHaveBeenCalled();
  });

  it('con sesión de Admin, carga y muestra los proyectos existentes', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(obrasService.getProyectosObraAdmin).mockResolvedValue([PROYECTO_MOCK]);

    render(<ObrasAdminPage />);

    expect(await screen.findByText('Interventoría casa campestre')).toBeInTheDocument();
    expect(screen.getByText(/1\/3 hitos completados/i)).toBeInTheDocument();
  });

  it('crear un proyecto muestra el link único para compartir con el cliente', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(obrasService.getProyectosObraAdmin).mockResolvedValue([]);
    vi.mocked(obrasService.crearProyectoObra).mockResolvedValue({ id: 'proy-nuevo', tokenAcceso: 'xyz789' });
    const user = userEvent.setup();

    render(<ObrasAdminPage />);
    await screen.findByText(/aún no hay proyectos/i);

    await user.type(screen.getByPlaceholderText(/nombre del cliente/i), 'Ana Restrepo');
    await user.type(screen.getByPlaceholderText(/email del cliente/i), 'ana@example.com');
    await user.type(screen.getByPlaceholderText(/teléfono del cliente/i), '3109876543');
    await user.type(screen.getByPlaceholderText(/nombre del proyecto/i), 'Interventoría casa campestre');
    await user.click(screen.getByRole('button', { name: /crear proyecto/i }));

    expect(await screen.findByText(/xyz789/)).toBeInTheDocument();
  });
});
