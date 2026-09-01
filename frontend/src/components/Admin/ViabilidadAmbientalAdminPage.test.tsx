import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ViabilidadAmbientalAdminPage } from './ViabilidadAmbientalAdminPage';
import * as viabilidadAmbientalService from '../../services/viabilidadAmbientalService';
import { ApiError } from '../../types/api';
import type { SolicitudViabilidadAmbientalListItem } from '../../types/viabilidadAmbiental';

vi.mock('../../services/viabilidadAmbientalService', async (importOriginal) => ({
  ...(await importOriginal<typeof viabilidadAmbientalService>()),
  listarSolicitudesViabilidadAmbiental: vi.fn(),
  confirmarPagoViabilidadAmbiental: vi.fn(),
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

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('ViabilidadAmbientalAdminPage', () => {
  it('pide el API key antes de mostrar cualquier dato', () => {
    render(<ViabilidadAmbientalAdminPage />);

    expect(screen.getByPlaceholderText(/x-admin-api-key/i)).toBeInTheDocument();
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).not.toHaveBeenCalled();
  });

  it('tras ingresar el key, carga y muestra la lista de solicitudes', async () => {
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockResolvedValue([SOLICITUD_MOCK]);
    const user = userEvent.setup();
    render(<ViabilidadAmbientalAdminPage />);

    await user.type(screen.getByPlaceholderText(/x-admin-api-key/i), 'mi-key-secreto');
    await user.click(screen.getByRole('button', { name: /entrar/i }));

    expect(await screen.findByText('Ana Restrepo')).toBeInTheDocument();
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).toHaveBeenCalledWith(
      'mi-key-secreto',
      undefined,
      expect.anything(),
    );
  });

  it('confirmar pago llama al servicio con el id y el apiKey, y recarga la lista', async () => {
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockResolvedValue([SOLICITUD_MOCK]);
    vi.mocked(viabilidadAmbientalService.confirmarPagoViabilidadAmbiental).mockResolvedValue({
      id: 'sol-123',
      estado: 'Pagada',
      pagoConfirmadoEn: '2026-09-01T12:05:00Z',
    });
    localStorage.setItem('admin-api-key', 'mi-key-secreto');
    const user = userEvent.setup();
    render(<ViabilidadAmbientalAdminPage />);

    await screen.findByText('Ana Restrepo');
    await user.click(screen.getByRole('button', { name: /confirmar pago/i }));

    expect(viabilidadAmbientalService.confirmarPagoViabilidadAmbiental).toHaveBeenCalledWith('sol-123', 'mi-key-secreto');
    expect(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).toHaveBeenCalledTimes(2);
  });

  it('con un 401 al cargar, limpia el key guardado y vuelve a pedirlo', async () => {
    vi.mocked(viabilidadAmbientalService.listarSolicitudesViabilidadAmbiental).mockRejectedValue(new ApiError(401));
    localStorage.setItem('admin-api-key', 'key-invalido');
    render(<ViabilidadAmbientalAdminPage />);

    expect(await screen.findByPlaceholderText(/x-admin-api-key/i)).toBeInTheDocument();
    expect(localStorage.getItem('admin-api-key')).toBeNull();
  });
});
