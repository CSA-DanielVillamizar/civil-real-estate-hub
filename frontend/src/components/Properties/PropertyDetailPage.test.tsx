import { render, screen } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { PropertyDetailPage } from './PropertyDetailPage';
import * as propertiesService from '../../services/propertiesService';
import { ApiError } from '../../types/api';
import type { PropertyDetailResponse } from '../../types/properties';
import { SITE_TITLE } from '../../seo';

vi.mock('../../services/propertiesService', async (importOriginal) => ({
  ...(await importOriginal<typeof propertiesService>()),
  getPropertyById: vi.fn(),
}));

const PROPERTY_MOCK: PropertyDetailResponse = {
  id: 'prop-1',
  titulo: 'Lote campestre',
  descripcion: 'Un lote con vista.',
  tipoInmueble: 'Lote',
  precio: 250_000_000,
  moneda: 'COP',
  direccion: 'Vereda La Primavera',
  municipio: 'Rionegro',
  departamento: 'Antioquia',
  areaTerrenoM2: 1200,
  pendientePorcentaje: 30,
  tipoSuelo: 'Franco',
  topografia: 'Inclinada',
  estado: 'Publicada',
  esViableConstructivamente: false,
  restriccionesViabilidad: ['Pendiente del terreno (30%) supera el máximo de referencia (25%).'],
  retirosAmbientales: [],
  multimedia: [],
};

describe('PropertyDetailPage', () => {
  beforeEach(() => {
    document.title = SITE_TITLE;
  });

  afterEach(() => {
    document.title = SITE_TITLE;
  });

  it('actualiza document.title con el título de la propiedad y lo restaura al desmontar', async () => {
    vi.mocked(propertiesService.getPropertyById).mockResolvedValue(PROPERTY_MOCK);
    const { unmount } = render(<PropertyDetailPage id="prop-1" />);

    await screen.findByText('Lote campestre');
    expect(document.title).toBe('Lote campestre | Plataforma Civil e Inmobiliaria');

    unmount();
    expect(document.title).toBe(SITE_TITLE);
  });

  it('muestra el detalle y las restricciones de viabilidad cuando la propiedad no es viable', async () => {
    vi.mocked(propertiesService.getPropertyById).mockResolvedValue(PROPERTY_MOCK);
    render(<PropertyDetailPage id="prop-1" />);

    expect(await screen.findByText('Lote campestre')).toBeInTheDocument();
    expect(screen.getByText(/supera el máximo de referencia/i)).toBeInTheDocument();
    expect(screen.getByText('¿Te interesa esta propiedad?')).toBeInTheDocument();
  });

  it('con un 404, muestra "no encontrada" en vez de un error genérico', async () => {
    vi.mocked(propertiesService.getPropertyById).mockRejectedValue(new ApiError(404));
    render(<PropertyDetailPage id="inexistente" />);

    expect(await screen.findByText(/propiedad no encontrada/i)).toBeInTheDocument();
  });
});
