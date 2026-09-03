import { render, screen } from '@testing-library/react';
import { afterEach, describe, expect, it } from 'vitest';
import App from './App';

afterEach(() => {
  window.history.pushState({}, '', '/');
});

describe('App — ruteo', () => {
  it('en una ruta desconocida, muestra la página 404 en vez del home', () => {
    window.history.pushState({}, '', '/esta-ruta-no-existe');

    render(<App />);

    expect(screen.getByText(/esta página no existe/i)).toBeInTheDocument();
    expect(screen.getByRole('link', { name: /volver al inicio/i })).toHaveAttribute('href', '/');
  });

  it('en "/", muestra el home (no la página 404)', () => {
    window.history.pushState({}, '', '/');

    render(<App />);

    expect(screen.queryByText(/esta página no existe/i)).not.toBeInTheDocument();
    expect(screen.getByText(/ingeniería y bienes raíces, en un solo lugar/i)).toBeInTheDocument();
  });

  it('en una ruta conocida, no muestra la página 404', () => {
    window.history.pushState({}, '', '/normativa');

    render(<App />);

    expect(screen.queryByText(/esta página no existe/i)).not.toBeInTheDocument();
  });
});
