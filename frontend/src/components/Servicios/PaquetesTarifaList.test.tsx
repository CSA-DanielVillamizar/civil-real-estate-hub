import { render, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { PaquetesTarifaList } from './PaquetesTarifaList';
import * as tarifasService from '../../services/tarifasService';
import { ServicioDeInteres } from '../../types/common';
import type { PaqueteTarifa } from '../../types/tarifas';

vi.mock('../../services/tarifasService', async (importOriginal) => ({
  ...(await importOriginal<typeof tarifasService>()),
  getPaquetesTarifaPublicados: vi.fn(),
}));

const CONSULTORIA_MOCK: PaqueteTarifa = {
  id: 'pt-1',
  servicioRelacionado: 'ConsultoriaYDisenoEstructural',
  titulo: 'Diseño estructural residencial',
  descripcion: 'Incluye planos y memoria de cálculo.',
  precioDesde: 50000,
  precioHasta: 80000,
  unidadPrecio: 'por m²',
  moneda: 'COP',
  publicado: true,
  creadoEn: '2026-09-03T12:00:00Z',
};

const INTERVENTORIA_MOCK: PaqueteTarifa = {
  ...CONSULTORIA_MOCK,
  id: 'pt-2',
  servicioRelacionado: 'InterventoriaYPresupuestos',
  titulo: 'Interventoría integral',
};

describe('PaquetesTarifaList', () => {
  it('muestra solo los paquetes del servicio pedido', async () => {
    vi.mocked(tarifasService.getPaquetesTarifaPublicados).mockResolvedValue([CONSULTORIA_MOCK, INTERVENTORIA_MOCK]);

    render(<PaquetesTarifaList servicio={ServicioDeInteres.ConsultoriaYDisenoEstructural} />);

    expect(await screen.findByText('Diseño estructural residencial')).toBeInTheDocument();
    expect(screen.queryByText('Interventoría integral')).not.toBeInTheDocument();
    expect(screen.getByText(/50.000.*80.000.*por m²/)).toBeInTheDocument();
  });

  it('sin paquetes para ese servicio, no renderiza nada', async () => {
    vi.mocked(tarifasService.getPaquetesTarifaPublicados).mockResolvedValue([INTERVENTORIA_MOCK]);

    const { container } = render(<PaquetesTarifaList servicio={ServicioDeInteres.ConsultoriaYDisenoEstructural} />);

    await waitFor(() => expect(tarifasService.getPaquetesTarifaPublicados).toHaveBeenCalled());
    expect(container).toBeEmptyDOMElement();
  });
});
