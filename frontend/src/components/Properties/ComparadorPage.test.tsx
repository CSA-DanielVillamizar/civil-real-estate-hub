import { render, screen } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ComparadorPage } from './ComparadorPage';
import * as propertiesService from '../../services/propertiesService';
import type { PropertyDetailResponse } from '../../types/properties';

vi.mock('../../services/propertiesService', async (importOriginal) => ({
  ...(await importOriginal<typeof propertiesService>()),
  getPropertyById: vi.fn(),
}));

function propiedad(overrides: Partial<PropertyDetailResponse>): PropertyDetailResponse {
  return {
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
    pendientePorcentaje: 15,
    tipoSuelo: 'Franco',
    topografia: 'Plana',
    estado: 'Publicada',
    esViableConstructivamente: true,
    restriccionesViabilidad: [],
    retirosAmbientales: [],
    multimedia: [],
    ...overrides,
  };
}

function setUrl(search: string) {
  window.history.pushState({}, '', `/comparar${search}`);
}

beforeEach(() => {
  vi.resetAllMocks();
});

afterEach(() => {
  window.history.pushState({}, '', '/');
});

describe('ComparadorPage', () => {
  it('con menos de 2 ids en la URL, invita a volver al catálogo en vez de intentar comparar', () => {
    setUrl('?ids=prop-1');
    render(<ComparadorPage />);

    expect(screen.getByText(/selecciona al menos 2 propiedades/i)).toBeInTheDocument();
    expect(propertiesService.getPropertyById).not.toHaveBeenCalled();
  });

  it('con 2 ids, pide el detalle de ambas y las muestra lado a lado', async () => {
    vi.mocked(propertiesService.getPropertyById).mockImplementation((id) =>
      Promise.resolve(
        id === 'prop-1'
          ? propiedad({ id: 'prop-1', titulo: 'Lote campestre', precio: 250_000_000 })
          : propiedad({ id: 'prop-2', titulo: 'Finca La Esperanza', precio: 400_000_000, pendientePorcentaje: 30 }),
      ),
    );
    setUrl('?ids=prop-1,prop-2');

    render(<ComparadorPage />);

    expect(await screen.findByText('Lote campestre')).toBeInTheDocument();
    expect(screen.getByText('Finca La Esperanza')).toBeInTheDocument();
    expect(screen.getByText('250.000.000 COP')).toBeInTheDocument();
    expect(screen.getByText('400.000.000 COP')).toBeInTheDocument();
    expect(propertiesService.getPropertyById).toHaveBeenCalledTimes(2);
  });
});
