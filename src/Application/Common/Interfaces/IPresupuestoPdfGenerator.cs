using Plataforma.Domain.Leads;

namespace Plataforma.Application.Common.Interfaces;

// Contrato declarado en Application, implementado en Infrastructure (QuestPDF)
// — la lógica de renderizado del PDF queda desacoplada del caso de uso y de
// la API; el handler no sabe (ni le importa) qué motor de PDF hay detrás.
public interface IPresupuestoPdfGenerator
{
    byte[] Generar(Lead lead);
}
