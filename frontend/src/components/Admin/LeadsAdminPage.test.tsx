import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { LeadsAdminPage } from './LeadsAdminPage';
import * as leadsService from '../../services/leadsService';
import * as authService from '../../services/authService';
import { ApiError } from '../../types/api';
import type { LeadListItem } from '../../types/leads';
import type { LoginResponse } from '../../types/auth';

vi.mock('../../services/leadsService', async (importOriginal) => ({
  ...(await importOriginal<typeof leadsService>()),
  getLeadsAdmin: vi.fn(),
  marcarLeadContactado: vi.fn(),
  calificarLead: vi.fn(),
  convertirLead: vi.fn(),
}));

vi.mock('../../services/authService', async (importOriginal) => ({
  ...(await importOriginal<typeof authService>()),
  login: vi.fn(),
}));

const LEAD_NUEVO: LeadListItem = {
  id: 'lead-1',
  nombre: 'Ana Restrepo',
  email: 'ana@example.com',
  telefono: '+573109876543',
  origen: 'FormularioContacto',
  estado: 'Nuevo',
  capturadoEn: '2026-09-02T12:00:00Z',
};

const SESION_ASESOR: LoginResponse = {
  token: 'token-jwt-asesor',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Laura Gómez',
  rol: 'AsesorComercial',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('LeadsAdminPage', () => {
  it('pide email y contraseña antes de mostrar cualquier lead', () => {
    render(<LeadsAdminPage />);

    expect(screen.getByPlaceholderText(/email/i)).toBeInTheDocument();
    expect(leadsService.getLeadsAdmin).not.toHaveBeenCalled();
  });

  it('un AsesorComercial autenticado sí ve este panel (rol acotado a Leads)', async () => {
    vi.mocked(authService.login).mockResolvedValue(SESION_ASESOR);
    vi.mocked(leadsService.getLeadsAdmin).mockResolvedValue([LEAD_NUEVO]);
    const user = userEvent.setup();
    render(<LeadsAdminPage />);

    await user.type(screen.getByPlaceholderText(/email/i), 'laura@example.com');
    await user.type(screen.getByPlaceholderText(/contraseña/i), 'clave-correcta');
    await user.click(screen.getByRole('button', { name: /entrar/i }));

    expect(await screen.findByText('Ana Restrepo')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /marcar contactado/i })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /^calificar$/i })).not.toBeInTheDocument();
  });

  it('marcar contactado llama al servicio con el id y el token, y recarga la lista', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ASESOR));
    vi.mocked(leadsService.getLeadsAdmin).mockResolvedValue([LEAD_NUEVO]);
    vi.mocked(leadsService.marcarLeadContactado).mockResolvedValue({ id: 'lead-1', estado: 'Contactado' });
    const user = userEvent.setup();
    render(<LeadsAdminPage />);

    await screen.findByText('Ana Restrepo');
    await user.click(screen.getByRole('button', { name: /marcar contactado/i }));

    expect(leadsService.marcarLeadContactado).toHaveBeenCalledWith('lead-1', 'token-jwt-asesor');
    expect(leadsService.getLeadsAdmin).toHaveBeenCalledTimes(2);
  });

  it('con un 401 al cargar, limpia la sesión guardada y vuelve a pedir login', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ASESOR));
    vi.mocked(leadsService.getLeadsAdmin).mockRejectedValue(new ApiError(401));
    render(<LeadsAdminPage />);

    expect(await screen.findByPlaceholderText(/email/i)).toBeInTheDocument();
    expect(localStorage.getItem('auth')).toBeNull();
  });
});
