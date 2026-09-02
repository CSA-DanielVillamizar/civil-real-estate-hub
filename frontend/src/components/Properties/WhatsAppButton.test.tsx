import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { WhatsAppButton } from './WhatsAppButton';

describe('WhatsAppButton', () => {
  it('arma el link de wa.me con el número configurado y el mensaje codificado', () => {
    render(<WhatsAppButton mensaje='Hola, me interesa la propiedad "Lote campestre".' />);

    const link = screen.getByRole('link', { name: /escríbenos por whatsapp/i });
    expect(link).toHaveAttribute(
      'href',
      'https://wa.me/573183507127?text=Hola%2C%20me%20interesa%20la%20propiedad%20%22Lote%20campestre%22.',
    );
    expect(link).toHaveAttribute('target', '_blank');
  });
});
