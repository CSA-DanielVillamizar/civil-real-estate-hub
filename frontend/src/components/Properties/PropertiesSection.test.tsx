import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { PropertiesSection } from './PropertiesSection';
import * as propertiesService from '../../services/propertiesService';
import type { PagedPropertyResponse, PropertyResponse } from '../../types/properties';

vi.mock('../../services/propertiesService', async (importOriginal) => ({
  ...(await importOriginal<typeof propertiesService>()),
  getProperties: vi.fn(),
}));

const PROPERTY_MOCK: PropertyResponse = {
  id: 'prop-1',
  titulo: 'Lote campestre',
  tipoInmueble: 'Lote',
  precio: 250_000_000,
  moneda: 'COP',
  municipio: 'Rionegro',
  departamento: 'Antioquia',
  areaTerrenoM2: 1200,
  estado: 'Publicada',
  esViableConstructivamente: true,
};

const PROPERTY_MOCK_2: PropertyResponse = { ...PROPERTY_MOCK, id: 'prop-2', titulo: 'Finca La Esperanza' };

function pagina(items: PropertyResponse[]): PagedPropertyResponse {
  return { items, page: 1, pageSize: 12, totalCount: items.length };
}

beforeEach(() => {
  vi.resetAllMocks();
});

describe('PropertiesSection', () => {
  it('muestra las propiedades devueltas por el servicio', async () => {
    vi.mocked(propertiesService.getProperties).mockResolvedValue(pagina([PROPERTY_MOCK]));
    render(<PropertiesSection />);

    expect(await screen.findByText('Lote campestre')).toBeInTheDocument();
    expect(screen.getByText('Viable constructivamente')).toBeInTheDocument();
  });

  it('sin resultados, muestra el mensaje de vacío en vez de romper', async () => {
    vi.mocked(propertiesService.getProperties).mockResolvedValue(pagina([]));
    render(<PropertiesSection />);

    expect(await screen.findByText(/no hay propiedades/i)).toBeInTheDocument();
  });

  it('cambiar el filtro de municipio vuelve a pedir la página 1 con ese filtro', async () => {
    vi.mocked(propertiesService.getProperties).mockResolvedValue(pagina([PROPERTY_MOCK]));
    const user = userEvent.setup();
    render(<PropertiesSection />);

    await screen.findByText('Lote campestre');
    await user.type(screen.getByLabelText(/municipio/i), 'Rionegro');

    expect(propertiesService.getProperties).toHaveBeenLastCalledWith(
      expect.objectContaining({ municipio: 'Rionegro', page: 1 }),
      expect.anything(),
    );
  });

  it('marcar 2 propiedades para comparar habilita el link "Comparar" con sus ids en la URL', async () => {
    vi.mocked(propertiesService.getProperties).mockResolvedValue(pagina([PROPERTY_MOCK, PROPERTY_MOCK_2]));
    const user = userEvent.setup();
    render(<PropertiesSection />);

    await screen.findByText('Lote campestre');

    expect(screen.queryByText(/seleccionadas para comparar/i)).not.toBeInTheDocument();

    const checkboxes = screen.getAllByRole('checkbox', { name: /comparar/i });
    await user.click(checkboxes[0]);
    expect(screen.getByText(/1 de 4 propiedades seleccionadas/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /^comparar$/i })).toBeDisabled();
    expect(screen.queryByRole('link', { name: /^comparar$/i })).not.toBeInTheDocument();

    await user.click(checkboxes[1]);
    expect(screen.getByText(/2 de 4 propiedades seleccionadas/i)).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /^comparar$/i })).toHaveAttribute('href', '/comparar?ids=prop-1,prop-2');
  });

  it('marcar una casilla no navega a la ficha de la propiedad', async () => {
    vi.mocked(propertiesService.getProperties).mockResolvedValue(pagina([PROPERTY_MOCK]));
    const user = userEvent.setup();
    render(<PropertiesSection />);

    await screen.findByText('Lote campestre');
    await user.click(screen.getByRole('checkbox', { name: /comparar/i }));

    expect(window.location.pathname).toBe('/');
  });
});
