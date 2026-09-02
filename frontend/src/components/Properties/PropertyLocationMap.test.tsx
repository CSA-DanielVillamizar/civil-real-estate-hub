import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { PropertyLocationMap } from './PropertyLocationMap';

describe('PropertyLocationMap', () => {
  it('sin latitud/longitud, no muestra nada', () => {
    const { container } = render(<PropertyLocationMap titulo="Lote campestre" />);

    expect(container).toBeEmptyDOMElement();
  });

  it('con latitud/longitud, muestra el iframe embebido con las coordenadas', () => {
    render(<PropertyLocationMap latitud={6.1548} longitud={-75.4319} titulo="Lote campestre" />);

    const iframe = screen.getByTitle('Ubicación de Lote campestre');
    expect(iframe).toHaveAttribute('src', 'https://www.google.com/maps?q=6.1548,-75.4319&output=embed');
  });
});
