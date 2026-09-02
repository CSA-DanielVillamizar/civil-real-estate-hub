import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { LeadsAdminPage } from './LeadsAdminPage';
import * as leadsService from '../../services/leadsService';
import { ApiError } from '../../types/api';
import type { LeadListItem } from '../../types/leads';

vi.mock('../../services/leadsService', async (importOriginal) => ({
  ...(await importOriginal<typeof leadsService>()),
  getLeadsAdmin: vi.fn(),
  marcarLeadContactado: vi.fn(),
  calificarLead: vi.fn(),
  convertirLead: vi.fn(),
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

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('LeadsAdminPage', () => {
  it('pide el API key antes de mostrar cualquier lead', () => {
    render(<LeadsAdminPage />);

    expect(screen.getByPlaceholderText(/x-admin-api-key/i)).toBeInTheDocument();
    expect(leadsService.getLeadsAdmin).not.toHaveBeenCalled();
  });

  it('tras ingresar el key, carga y muestra la lista con el botón de acción correcto para el estado', async () => {
    vi.mocked(leadsService.getLeadsAdmin).mockResolvedValue([LEAD_NUEVO]);
    const user = userEvent.setup();
    render(<LeadsAdminPage />);

    await user.type(screen.getByPlaceholderText(/x-admin-api-key/i), 'mi-key');
    await user.click(screen.getByRole('button', { name: /entrar/i }));

    expect(await screen.findByText('Ana Restrepo')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /marcar contactado/i })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /^calificar$/i })).not.toBeInTheDocument();
  });

  it('marcar contactado llama al servicio con el id y el apiKey, y recarga la lista', async () => {
    vi.mocked(leadsService.getLeadsAdmin).mockResolvedValue([LEAD_NUEVO]);
    vi.mocked(leadsService.marcarLeadContactado).mockResolvedValue({ id: 'lead-1', estado: 'Contactado' });
    localStorage.setItem('admin-api-key', 'mi-key');
    const user = userEvent.setup();
    render(<LeadsAdminPage />);

    await screen.findByText('Ana Restrepo');
    await user.click(screen.getByRole('button', { name: /marcar contactado/i }));

    expect(leadsService.marcarLeadContactado).toHaveBeenCalledWith('lead-1', 'mi-key');
    expect(leadsService.getLeadsAdmin).toHaveBeenCalledTimes(2);
  });

  it('con un 401 al cargar, limpia el key guardado y vuelve a pedirlo', async () => {
    vi.mocked(leadsService.getLeadsAdmin).mockRejectedValue(new ApiError(401));
    localStorage.setItem('admin-api-key', 'key-invalido');
    render(<LeadsAdminPage />);

    expect(await screen.findByPlaceholderText(/x-admin-api-key/i)).toBeInTheDocument();
    expect(localStorage.getItem('admin-api-key')).toBeNull();
  });
});
