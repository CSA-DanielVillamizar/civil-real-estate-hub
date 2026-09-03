import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ConfianzaAdminPage } from './ConfianzaAdminPage';
import * as confianzaService from '../../services/confianzaService';
import type { ContenidoConfianza } from '../../types/confianza';

vi.mock('../../services/confianzaService', async (importOriginal) => ({
  ...(await importOriginal<typeof confianzaService>()),
  getContenidoConfianzaAdmin: vi.fn(),
  crearContenidoConfianza: vi.fn(),
  publicarContenidoConfianza: vi.fn(),
  despublicarContenidoConfianza: vi.fn(),
}));

const SESION_ADMIN = {
  token: 'token-jwt-admin',
  expiraEn: '2099-01-01T00:00:00Z',
  nombre: 'Daniel Villamizar',
  rol: 'Admin',
};

const CONTENIDO_MOCK: ContenidoConfianza = {
  id: 'cc-1',
  tipo: 'Testimonio',
  titulo: 'Ana Restrepo',
  descripcion: 'Excelente trabajo.',
  municipio: 'Rionegro',
  servicioRelacionado: 'ConsultoriaYDisenoEstructural',
  publicado: false,
  creadoEn: '2026-09-03T12:00:00Z',
};

beforeEach(() => {
  vi.resetAllMocks();
  localStorage.clear();
});

describe('ConfianzaAdminPage', () => {
  it('con sesión de Admin, carga y muestra el contenido existente', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(confianzaService.getContenidoConfianzaAdmin).mockResolvedValue([CONTENIDO_MOCK]);

    render(<ConfianzaAdminPage />);

    expect(await screen.findByText('Ana Restrepo')).toBeInTheDocument();
    // Texto exacto (no regex case-insensitive): el botón "Crear (sin
    // publicar)" del formulario también contiene la subcadena "sin publicar".
    expect(screen.getByText('Sin publicar')).toBeInTheDocument();
  });

  it('publicar un contenido llama al servicio con su id', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(confianzaService.getContenidoConfianzaAdmin).mockResolvedValue([CONTENIDO_MOCK]);
    vi.mocked(confianzaService.publicarContenidoConfianza).mockResolvedValue({ ...CONTENIDO_MOCK, publicado: true });
    const user = userEvent.setup();

    render(<ConfianzaAdminPage />);
    await screen.findByText('Ana Restrepo');
    await user.click(screen.getByRole('button', { name: /^publicar$/i }));

    expect(confianzaService.publicarContenidoConfianza).toHaveBeenCalledWith('cc-1', 'token-jwt-admin');
  });

  it('crear un testimonio nuevo llama al servicio con los datos del formulario', async () => {
    localStorage.setItem('auth', JSON.stringify(SESION_ADMIN));
    vi.mocked(confianzaService.getContenidoConfianzaAdmin).mockResolvedValue([]);
    vi.mocked(confianzaService.crearContenidoConfianza).mockResolvedValue(CONTENIDO_MOCK);
    const user = userEvent.setup();

    render(<ConfianzaAdminPage />);
    await screen.findByText(/todavía no hay testimonios/i);

    await user.type(screen.getByPlaceholderText(/nombre del cliente/i), 'Ana Restrepo');
    await user.type(screen.getByPlaceholderText(/la cita del cliente/i), 'Excelente trabajo.');
    await user.click(screen.getByRole('button', { name: /crear \(sin publicar\)/i }));

    expect(confianzaService.crearContenidoConfianza).toHaveBeenCalledWith(
      {
        tipo: 'Testimonio',
        titulo: 'Ana Restrepo',
        descripcion: 'Excelente trabajo.',
        municipio: undefined,
        servicioRelacionado: 'ConsultoriaYDisenoEstructural',
      },
      'token-jwt-admin',
    );
    expect(await screen.findByText(/creado sin publicar/i)).toBeInTheDocument();
  });
});
