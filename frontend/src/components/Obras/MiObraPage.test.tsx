import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { MiObraPage } from './MiObraPage';
import * as obrasService from '../../services/obrasService';
import { ApiError } from '../../types/api';
import type { ProyectoObraDetalle } from '../../types/obras';

vi.mock('../../services/obrasService', async (importOriginal) => ({
  ...(await importOriginal<typeof obrasService>()),
  getProyectoObraPorToken: vi.fn(),
}));

const PROYECTO_MOCK: ProyectoObraDetalle = {
  id: 'proy-1',
  nombreCliente: 'Ana Restrepo',
  emailCliente: 'ana@example.com',
  telefonoCliente: '+573109876543',
  nombreProyecto: 'Interventoría casa campestre',
  estado: 'EnEjecucion',
  creadoEn: '2026-09-01T12:00:00Z',
  hitos: [
    { id: 'h1', nombre: 'Cimentación', orden: 0, estado: 'Completado', fechaCompletado: '2026-09-02T12:00:00Z' },
    { id: 'h2', nombre: 'Estructura', orden: 1, estado: 'EnProgreso' },
    { id: 'h3', nombre: 'Acabados', orden: 2, estado: 'Pendiente' },
  ],
};

describe('MiObraPage', () => {
  it('con un token válido, muestra el nombre del proyecto y el progreso de sus hitos', async () => {
    vi.mocked(obrasService.getProyectoObraPorToken).mockResolvedValue(PROYECTO_MOCK);

    render(<MiObraPage token="token-valido" />);

    expect(await screen.findByText('Interventoría casa campestre')).toBeInTheDocument();
    expect(screen.getByText(/1 de 3 hitos completados/i)).toBeInTheDocument();
    expect(screen.getByText('Cimentación')).toBeInTheDocument();
    expect(screen.getByText('Estructura')).toBeInTheDocument();
    expect(screen.getByText('Acabados')).toBeInTheDocument();
  });

  it('con un token inexistente, muestra "link no válido" en vez de un error genérico', async () => {
    vi.mocked(obrasService.getProyectoObraPorToken).mockRejectedValue(new ApiError(404));

    render(<MiObraPage token="token-invalido" />);

    expect(await screen.findByText(/link no válido/i)).toBeInTheDocument();
  });
});
