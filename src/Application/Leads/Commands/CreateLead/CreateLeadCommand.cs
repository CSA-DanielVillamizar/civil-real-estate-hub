using MediatR;
using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.CreateLead;

// Recibe primitivos (no Value Objects de Domain): la construcción/validación de
// invariantes (Email, Telefono, DatosCalculoObra) ocurre en el handler, después
// de que CreateLeadCommandValidator valida la forma primitiva del comando.
public sealed record CreateLeadCommand(
    string Nombre,
    string Email,
    string Telefono,
    string? Indicativo,
    OrigenLead Origen,
    Guid? PropiedadDeInteresId,
    DatosCalculoObraInput? DatosCalculoObra
) : IRequest<CreateLeadResult>;
