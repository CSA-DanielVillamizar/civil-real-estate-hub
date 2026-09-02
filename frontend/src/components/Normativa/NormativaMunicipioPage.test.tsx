import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { NormativaMunicipioPage } from './NormativaMunicipioPage';

describe('NormativaMunicipioPage', () => {
  it('con un slug válido, muestra el POT, quién expide licencias y sus fuentes', () => {
    render(<NormativaMunicipioPage slug="rionegro" />);

    expect(screen.getByRole('heading', { name: /normativa de construcción en rionegro/i })).toBeInTheDocument();
    expect(screen.getByText(/curaduría urbana primera y curaduría urbana segunda/i)).toBeInTheDocument();
    expect(screen.getAllByText(/fuente:/i).length).toBeGreaterThan(0);
  });

  it('con un slug inexistente, muestra "no encontrado" en vez de romper', () => {
    render(<NormativaMunicipioPage slug="municipio-inventado" />);

    expect(screen.getByText(/municipio no encontrado/i)).toBeInTheDocument();
  });
});
