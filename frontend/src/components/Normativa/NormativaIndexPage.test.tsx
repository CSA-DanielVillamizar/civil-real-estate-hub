import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { NormativaIndexPage } from './NormativaIndexPage';
import { MUNICIPIOS } from '../../data/municipios';

describe('NormativaIndexPage', () => {
  it('lista los 5 municipios con un link a su página de detalle', () => {
    render(<NormativaIndexPage />);

    expect(MUNICIPIOS).toHaveLength(5);
    for (const m of MUNICIPIOS) {
      const link = screen.getByRole('link', { name: new RegExp(m.nombre) });
      expect(link).toHaveAttribute('href', `/normativa/${m.slug}`);
    }
  });
});
