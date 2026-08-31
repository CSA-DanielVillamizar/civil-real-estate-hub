using MediatR;
using Plataforma.Application.Leads.Commands.CreateLead;

namespace Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf;

// DatosCalculoObra es nullable (igual que en CreateLeadCommand) para que la
// construcción del comando nunca falle por un input incompleto — es
// GenerarPresupuestoPdfCommandValidator quien lo exige vía NotNull() y lo
// reporta como 400 a través del pipeline de MediatR, no una excepción cruda
// al mapear el request.
public sealed record GenerarPresupuestoPdfCommand(
    string Nombre,
    string Email,
    string Telefono,
    string? Indicativo,
    Guid? PropiedadDeInteresId,
    DatosCalculoObraInput? DatosCalculoObra
) : IRequest<GenerarPresupuestoPdfResult>;
