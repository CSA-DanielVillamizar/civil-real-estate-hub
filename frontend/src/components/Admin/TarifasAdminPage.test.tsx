import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { TarifasAdminPage } from './TarifasAdminPage';
import * as tarifasService from '../../services/tarifasService';
import type { PaqueteTarifa } from '../../types/tarifas';

vi.mock('../../services/tarifasService', async (importOriginal) => ({
  ...(await importOriginal<typeof tarifasService>()),
  getPaquetesTarifaAdmin: vi.fn(),
  crearPaqueteTarifa: vi.fn(),
  publicarPaqueteTarifa: vi.fn(),
  despublicarPaqueteTarifa: vi.fn(),
}));

const SESION_ADMIN = {
  token: 'token-jwt-admin',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Daniel Villamizar',
  rol: 'Admin',
};

const PAQUETE_MOCK: PaqueteTarifa = {
  id: 'pt-1',
  servicioRelacionado: 'ConsultoriaYDisenoEstructural',
  titulo: 'Diseño estructural residencial',
  descripcion: 'Incluye planos y memoria de cálculo.',
  precioDesde: 50000,
  precioHasta: 80000,
  unidadPrecio: 'por m²',
  moneda: 'COP',
  publicado: false,
  creadoEn: '2026-09-03T12:00:00Z',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('TarifasAdminPage', () => {
  it('con sesión de Admin, carga y muestra los paquetes de tarifa existentes', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(tarifasService.getPaquetesTarifaAdmin).mockResolvedValue([PAQUETE_MOCK]);

    render(<TarifasAdminPage />);

    expect(await screen.findByText('Diseño estructural residencial')).toBeInTheDocument();
    expect(screen.getByText('Sin publicar')).toBeInTheDocument();
    expect(screen.getByText(/50.000.*80.000.*por m²/)).toBeInTheDocument();
  });

  it('publicar un paquete llama al servicio con su id', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(tarifasService.getPaquetesTarifaAdmin).mockResolvedValue([PAQUETE_MOCK]);
    vi.mocked(tarifasService.publicarPaqueteTarifa).mockResolvedValue({ ...PAQUETE_MOCK, publicado: true });
    const user = userEvent.setup();

    render(<TarifasAdminPage />);
    await screen.findByText('Diseño estructural residencial');
    await user.click(screen.getByRole('button', { name: /^publicar$/i }));

    expect(tarifasService.publicarPaqueteTarifa).toHaveBeenCalledWith('pt-1', 'token-jwt-admin');
  });

  it('crear un paquete nuevo llama al servicio con los datos del formulario', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(tarifasService.getPaquetesTarifaAdmin).mockResolvedValue([]);
    vi.mocked(tarifasService.crearPaqueteTarifa).mockResolvedValue(PAQUETE_MOCK);
    const user = userEvent.setup();

    render(<TarifasAdminPage />);
    await screen.findByText(/todavía no hay paquetes de tarifa/i);

    await user.type(screen.getByPlaceholderText(/nombre del paquete/i), 'Diseño estructural residencial');
    await user.type(screen.getByPlaceholderText(/qué incluye/i), 'Incluye planos y memoria de cálculo.');
    await user.type(screen.getByPlaceholderText(/unidad \(ej\./i), 'por m²');
    await user.click(screen.getByRole('button', { name: /crear \(sin publicar\)/i }));

    expect(tarifasService.crearPaqueteTarifa).toHaveBeenCalledWith(
      {
        servicioRelacionado: 'ConsultoriaYDisenoEstructural',
        titulo: 'Diseño estructural residencial',
        descripcion: 'Incluye planos y memoria de cálculo.',
        precioDesde: undefined,
        precioHasta: undefined,
        unidadPrecio: 'por m²',
        moneda: 'COP',
      },
      'token-jwt-admin',
    );
    expect(await screen.findByText(/creado sin publicar/i)).toBeInTheDocument();
  });
});
