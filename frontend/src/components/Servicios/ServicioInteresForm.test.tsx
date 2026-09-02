import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { ServicioInteresForm } from './ServicioInteresForm';
import * as leadsService from '../../services/leadsService';
import { OrigenLead, ServicioDeInteres } from '../../types/common';
import type { CreateLeadResponse } from '../../types/leads';

vi.mock('../../services/leadsService');

const LEAD_RESPONSE_MOCK: CreateLeadResponse = { id: 'lead-123', estado: 'Nuevo' };

beforeEach(() => {
  vi.resetAllMocks();
});

describe('ServicioInteresForm', () => {
  it('envía el lead con el servicioDeInteres fijo de la sección y el mensaje libre', async () => {
    vi.mocked(leadsService.createLead).mockResolvedValue(LEAD_RESPONSE_MOCK);
    const user = userEvent.setup();
    render(<ServicioInteresForm servicio={ServicioDeInteres.InterventoriaYPresupuestos} />);

    await user.type(screen.getByLabelText(/nombre/i), 'Ana Restrepo');
    await user.type(screen.getByLabelText(/email/i), 'ana@example.com');
    await user.type(screen.getByLabelText(/teléfono/i), '3109876543');
    await user.type(screen.getByLabelText(/cuéntanos tu proyecto/i), 'Lote de 800m² en Rionegro.');
    await user.click(screen.getByRole('button', { name: /solicitar contacto/i }));

    expect(leadsService.createLead).toHaveBeenCalledWith({
      nombre: 'Ana Restrepo',
      email: 'ana@example.com',
      telefono: '3109876543',
      origen: OrigenLead.FormularioContacto,
      servicioDeInteres: ServicioDeInteres.InterventoriaYPresupuestos,
      mensaje: 'Lote de 800m² en Rionegro.',
    });
    expect(await screen.findByText(/gracias/i)).toBeInTheDocument();
  });

  it('no envía el formulario si el email es inválido', async () => {
    const user = userEvent.setup();
    render(<ServicioInteresForm servicio={ServicioDeInteres.ConsultoriaYDisenoEstructural} />);

    await user.type(screen.getByLabelText(/nombre/i), 'Ana Restrepo');
    await user.type(screen.getByLabelText(/email/i), 'no-es-un-email');
    await user.type(screen.getByLabelText(/teléfono/i), '3109876543');
    await user.click(screen.getByRole('button', { name: /solicitar contacto/i }));

    expect(screen.getByText(/ingresa un correo válido/i)).toBeInTheDocument();
    expect(leadsService.createLead).not.toHaveBeenCalled();
  });
});
