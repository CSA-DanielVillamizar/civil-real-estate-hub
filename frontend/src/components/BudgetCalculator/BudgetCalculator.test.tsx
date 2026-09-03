import { fireEvent, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { BudgetCalculator } from './BudgetCalculator';
import * as apiClient from '../../services/apiClient';
import * as budgetsService from '../../services/budgetsService';
import * as leadsService from '../../services/leadsService';
import type { EstimacionCosto } from '../../types/common';
import type { CreateLeadResponse } from '../../types/leads';

// Se mockea la capa de servicios (no fetch directamente): aísla el componente
// y sus hooks del transporte HTTP, que ya se prueba en services/*.ts.
vi.mock('../../services/budgetsService');
vi.mock('../../services/leadsService');
// Mock parcial: descargarBlob usa URL.createObjectURL, que jsdom no
// implementa — el resto de apiClient (buildQueryString, etc.) queda intacto.
vi.mock('../../services/apiClient', async (importOriginal) => ({
  ...(await importOriginal<typeof apiClient>()),
  descargarBlob: vi.fn(),
}));

const ESTIMACION_MOCK: EstimacionCosto = {
  montoMinimo: 162_000_000,
  montoMaximo: 207_000_000,
  moneda: 'COP',
  desglose: [
    { categoria: 'ManoDeObra', monto: 63_000_000 },
    { categoria: 'Materiales', monto: 81_000_000 },
    { categoria: 'Equipos', monto: 18_000_000 },
    { categoria: 'AdministracionYUtilidad', monto: 18_000_000 },
  ],
};

const LEAD_RESPONSE_MOCK: CreateLeadResponse = { id: 'lead-123', estado: 'Nuevo' };

beforeEach(() => {
  vi.resetAllMocks();
});

// Llena el paso 1 (calculadora) con datos válidos y envía — usa fireEvent en el
// input numérico porque userEvent.type carácter-por-carácter es conocido por
// comportarse de forma inestable con <input type="number"> en jsdom.
async function completarPaso1YAvanzar(user: ReturnType<typeof userEvent.setup>) {
  fireEvent.change(screen.getByLabelText(/área de construcción/i), { target: { value: '100' } });
  await user.selectOptions(screen.getByLabelText(/tipo de proyecto/i), 'Vivienda');
  await user.selectOptions(screen.getByLabelText(/nivel de acabado/i), 'Basico');
  await user.type(screen.getByLabelText(/municipio/i), 'Gómez Plata');
  await user.click(screen.getByRole('button', { name: /calcular estimado/i }));
}

describe('BudgetCalculator', () => {
  it('muestra un error de validación cuando el área de construcción está vacía', async () => {
    const user = userEvent.setup();
    render(<BudgetCalculator />);

    await user.click(screen.getByRole('button', { name: /calcular estimado/i }));

    expect(await screen.findByText(/ingresa el área de construcción/i)).toBeInTheDocument();
    expect(budgetsService.calculateBudget).not.toHaveBeenCalled();
  });

  it('muestra un error de validación cuando el área de construcción es negativa', async () => {
    const user = userEvent.setup();
    render(<BudgetCalculator />);

    fireEvent.change(screen.getByLabelText(/área de construcción/i), { target: { value: '-10' } });
    await user.selectOptions(screen.getByLabelText(/tipo de proyecto/i), 'Vivienda');
    await user.selectOptions(screen.getByLabelText(/nivel de acabado/i), 'Medio');
    await user.type(screen.getByLabelText(/municipio/i), 'Gómez Plata');
    await user.click(screen.getByRole('button', { name: /calcular estimado/i }));

    expect(await screen.findByText(/el área debe ser un número mayor que 0/i)).toBeInTheDocument();
    expect(budgetsService.calculateBudget).not.toHaveBeenCalled();
  });

  it('avanza al paso 2 (estimado + captura de lead) cuando el cálculo se completa', async () => {
    vi.mocked(budgetsService.calculateBudget).mockResolvedValue(ESTIMACION_MOCK);
    const user = userEvent.setup();
    render(<BudgetCalculator />);

    await completarPaso1YAvanzar(user);

    expect(await screen.findByText(/estimado de inversión/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/nombre completo/i)).toBeInTheDocument();
    expect(budgetsService.calculateBudget).toHaveBeenCalledWith(
      { areaConstruccionM2: 100, tipoAcabado: 'Basico', municipio: 'Gómez Plata', tipoProyecto: 'Vivienda' },
      expect.anything(),
    );
  });

  it('llama a createLead con el payload correcto al enviar el formulario final', async () => {
    vi.mocked(budgetsService.calculateBudget).mockResolvedValue(ESTIMACION_MOCK);
    vi.mocked(leadsService.createLead).mockResolvedValue(LEAD_RESPONSE_MOCK);
    const user = userEvent.setup();
    render(<BudgetCalculator />);

    await completarPaso1YAvanzar(user);
    await screen.findByText(/estimado de inversión/i);

    await user.type(screen.getByLabelText(/nombre completo/i), 'Ana Restrepo');
    await user.type(screen.getByLabelText(/correo electrónico/i), 'ana@example.com');
    await user.type(screen.getByLabelText(/teléfono/i), '3109876543');
    await user.click(screen.getByRole('checkbox', { name: /acepto/i }));
    await user.click(screen.getByRole('button', { name: /quiero mi cotización detallada/i }));

    expect(await screen.findByText(/ya recibimos tu solicitud/i)).toBeInTheDocument();
    expect(leadsService.createLead).toHaveBeenCalledWith({
      nombre: 'Ana Restrepo',
      email: 'ana@example.com',
      telefono: '3109876543',
      origen: 'CalculadoraObra',
      datosCalculoObra: {
        areaConstruccionM2: 100,
        tipoAcabado: 'Basico',
        municipio: 'Gómez Plata',
        tipoProyecto: 'Vivienda',
      },
    });
  });

  it('descarga el PDF con el payload correcto al hacer clic en "Descargar presupuesto en PDF"', async () => {
    vi.mocked(budgetsService.calculateBudget).mockResolvedValue(ESTIMACION_MOCK);
    const pdfBlob = new Blob(['contenido-pdf'], { type: 'application/pdf' });
    vi.mocked(leadsService.generarPresupuestoPdf).mockResolvedValue({ blob: pdfBlob, fileName: 'presupuesto.pdf' });
    const user = userEvent.setup();
    render(<BudgetCalculator />);

    await completarPaso1YAvanzar(user);
    await screen.findByText(/estimado de inversión/i);

    await user.type(screen.getByLabelText(/nombre completo/i), 'Carlos Mendez');
    await user.type(screen.getByLabelText(/correo electrónico/i), 'carlos@example.com');
    await user.type(screen.getByLabelText(/teléfono/i), '3157894561');
    await user.click(screen.getByRole('checkbox', { name: /acepto/i }));
    await user.click(screen.getByRole('button', { name: /descargar presupuesto en pdf/i }));

    expect(await screen.findByText(/tu pdf se está descargando/i)).toBeInTheDocument();
    expect(leadsService.generarPresupuestoPdf).toHaveBeenCalledWith({
      nombre: 'Carlos Mendez',
      email: 'carlos@example.com',
      telefono: '3157894561',
      origen: 'CalculadoraObra',
      datosCalculoObra: {
        areaConstruccionM2: 100,
        tipoAcabado: 'Basico',
        municipio: 'Gómez Plata',
        tipoProyecto: 'Vivienda',
      },
    });
    expect(apiClient.descargarBlob).toHaveBeenCalledWith(pdfBlob, 'presupuesto.pdf');
  });
});
