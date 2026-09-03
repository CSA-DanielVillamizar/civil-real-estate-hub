import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ViabilidadAmbientalSection } from './ViabilidadAmbientalSection';
import * as viabilidadAmbientalService from '../../services/viabilidadAmbientalService';
import { ApiError } from '../../types/api';
import type { SolicitarViabilidadAmbientalResponse } from '../../types/viabilidadAmbiental';

vi.mock('../../services/viabilidadAmbientalService', async (importOriginal) => ({
  ...(await importOriginal<typeof viabilidadAmbientalService>()),
  solicitarViabilidadAmbiental: vi.fn(),
}));

const RESPUESTA_MOCK: SolicitarViabilidadAmbientalResponse = {
  id: 'sol-123',
  estado: 'Solicitada',
  monto: 200_000,
  moneda: 'COP',
  datosBancarios: { banco: 'Bancolombia', tipoCuenta: 'Ahorros', numeroCuenta: '12345678', titularCuenta: 'Plataforma SAS', qrImageUrl: '' },
};

beforeEach(() => {
  vi.resetAllMocks();
});

async function completarYEnviar(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/nombre completo/i), 'Ana Restrepo');
  await user.type(screen.getByLabelText(/correo electrónico/i), 'ana@example.com');
  await user.type(screen.getByLabelText(/teléfono/i), '3109876543');
  await user.type(screen.getByLabelText(/departamento/i), 'Antioquia');
  await user.type(screen.getByLabelText(/municipio/i), 'Rionegro');
  await user.click(screen.getByRole('checkbox', { name: /acepto/i }));
  await user.click(screen.getByRole('button', { name: /solicitar estudio de viabilidad ambiental/i }));
}

describe('ViabilidadAmbientalSection', () => {
  it('muestra errores de validación cuando el formulario está vacío', async () => {
    const user = userEvent.setup();
    render(<ViabilidadAmbientalSection />);

    await user.click(screen.getByRole('button', { name: /solicitar estudio de viabilidad ambiental/i }));

    expect(await screen.findByText(/ingresa tu nombre/i)).toBeInTheDocument();
    expect(viabilidadAmbientalService.solicitarViabilidadAmbiental).not.toHaveBeenCalled();
  });

  it('con datos válidos, envía la solicitud y muestra las instrucciones de pago', async () => {
    vi.mocked(viabilidadAmbientalService.solicitarViabilidadAmbiental).mockResolvedValue(RESPUESTA_MOCK);
    const user = userEvent.setup();
    render(<ViabilidadAmbientalSection />);

    await completarYEnviar(user);

    expect(await screen.findByText('Solicitud registrada')).toBeInTheDocument();
    expect(screen.getByText('12345678')).toBeInTheDocument();
    expect(viabilidadAmbientalService.solicitarViabilidadAmbiental).toHaveBeenCalledWith(
      expect.objectContaining({ nombre: 'Ana Restrepo', departamento: 'Antioquia', municipio: 'Rionegro' }),
    );
  });

  it('cuando los datos bancarios llegan vacíos, muestra el aviso en vez de campos en blanco', async () => {
    vi.mocked(viabilidadAmbientalService.solicitarViabilidadAmbiental).mockResolvedValue({
      ...RESPUESTA_MOCK,
      datosBancarios: { banco: '', tipoCuenta: '', numeroCuenta: '', titularCuenta: '', qrImageUrl: '' },
    });
    const user = userEvent.setup();
    render(<ViabilidadAmbientalSection />);

    await completarYEnviar(user);

    expect(await screen.findByText(/estamos terminando de publicar/i)).toBeInTheDocument();
  });

  it('con un error del servidor, lo muestra sin romper el formulario', async () => {
    vi.mocked(viabilidadAmbientalService.solicitarViabilidadAmbiental).mockRejectedValue(
      new ApiError(500, { title: 'Ocurrió un error inesperado.' }),
    );
    const user = userEvent.setup();
    render(<ViabilidadAmbientalSection />);

    await completarYEnviar(user);

    expect(await screen.findByText(/ocurrió un error inesperado/i)).toBeInTheDocument();
  });
});
