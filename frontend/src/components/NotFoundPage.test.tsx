import { render, screen } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { NotFoundPage } from './NotFoundPage';
import { SITE_TITLE } from '../seo';

describe('NotFoundPage', () => {
  beforeEach(() => {
    document.title = SITE_TITLE;
  });

  afterEach(() => {
    document.title = SITE_TITLE;
  });

  it('actualiza document.title y lo restaura al desmontar', () => {
    const { unmount } = render(<NotFoundPage />);

    expect(document.title).toBe(`Página no encontrada | ${SITE_TITLE}`);

    unmount();
    expect(document.title).toBe(SITE_TITLE);
  });

  it('muestra links para volver al inicio y ver propiedades', () => {
    render(<NotFoundPage />);

    expect(screen.getByRole('link', { name: /volver al inicio/i })).toHaveAttribute('href', '/');
    expect(screen.getByRole('link', { name: /ver propiedades/i })).toHaveAttribute('href', '/#propiedades');
  });
});
