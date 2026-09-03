import { render, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { ConfianzaSection } from './ConfianzaSection';
import * as confianzaService from '../../services/confianzaService';
import type { ContenidoConfianza } from '../../types/confianza';

vi.mock('../../services/confianzaService', async (importOriginal) => ({
  ...(await importOriginal<typeof confianzaService>()),
  getContenidoConfianzaPublicado: vi.fn(),
}));

const TESTIMONIO_MOCK: ContenidoConfianza = {
  id: 'cc-1',
  tipo: 'Testimonio',
  titulo: 'Ana Restrepo',
  descripcion: 'Excelente trabajo con el diseño estructural.',
  municipio: 'Rionegro',
  servicioRelacionado: 'ConsultoriaYDisenoEstructural',
  publicado: true,
  creadoEn: '2026-09-03T12:00:00Z',
};

const PORTAFOLIO_MOCK: ContenidoConfianza = {
  id: 'cc-2',
  tipo: 'Portafolio',
  titulo: 'Bodega industrial La Ceja',
  descripcion: 'Interventoría completa de la obra.',
  municipio: 'La Ceja',
  servicioRelacionado: 'InterventoriaYPresupuestos',
  publicado: true,
  creadoEn: '2026-09-03T12:00:00Z',
};

describe('ConfianzaSection', () => {
  it('muestra los testimonios y el portafolio publicados, separados por sección', async () => {
    vi.mocked(confianzaService.getContenidoConfianzaPublicado).mockResolvedValue([TESTIMONIO_MOCK, PORTAFOLIO_MOCK]);

    render(<ConfianzaSection />);

    expect(await screen.findByText(/lo que dicen nuestros clientes/i)).toBeInTheDocument();
    expect(screen.getByText(/excelente trabajo con el diseño estructural/i)).toBeInTheDocument();
    expect(screen.getByText(/proyectos entregados/i)).toBeInTheDocument();
    expect(screen.getByText('Bodega industrial La Ceja')).toBeInTheDocument();
  });

  it('sin contenido publicado, no renderiza nada', async () => {
    vi.mocked(confianzaService.getContenidoConfianzaPublicado).mockResolvedValue([]);

    const { container } = render(<ConfianzaSection />);

    await waitFor(() => expect(confianzaService.getContenidoConfianzaPublicado).toHaveBeenCalled());
    expect(container).toBeEmptyDOMElement();
  });
});
