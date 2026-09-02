import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it } from 'vitest';
import { PhotoGallery } from './PhotoGallery';
import type { ArchivoMultimedia } from '../../types/properties';

const FOTOS: ArchivoMultimedia[] = [
  { id: 'f1', url: 'https://example.com/1.jpg', tipo: 'Foto', orden: 0 },
  { id: 'f2', url: 'https://example.com/2.jpg', tipo: 'Foto', orden: 1 },
];

describe('PhotoGallery', () => {
  it('sin fotos, muestra el placeholder en vez de la grilla', () => {
    render(<PhotoGallery fotos={[]} titulo="Lote campestre" />);

    expect(screen.getByText(/sin fotos disponibles/i)).toBeInTheDocument();
  });

  it('al hacer clic en una foto, abre el lightbox a pantalla completa', async () => {
    const user = userEvent.setup();
    render(<PhotoGallery fotos={FOTOS} titulo="Lote campestre" />);

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();

    await user.click(screen.getByAltText('Lote campestre — foto 1'));

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('1 / 2')).toBeInTheDocument();
  });

  it('el botón "siguiente" avanza a la foto 2', async () => {
    const user = userEvent.setup();
    render(<PhotoGallery fotos={FOTOS} titulo="Lote campestre" />);

    await user.click(screen.getByAltText('Lote campestre — foto 1'));
    await user.click(screen.getByRole('button', { name: /foto siguiente/i }));

    expect(screen.getByText('2 / 2')).toBeInTheDocument();
  });

  it('la tecla Escape cierra el lightbox', async () => {
    const user = userEvent.setup();
    render(<PhotoGallery fotos={FOTOS} titulo="Lote campestre" />);

    await user.click(screen.getByAltText('Lote campestre — foto 1'));
    expect(screen.getByRole('dialog')).toBeInTheDocument();

    await user.keyboard('{Escape}');

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });
});
