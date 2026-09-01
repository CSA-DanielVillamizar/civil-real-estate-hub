using MediatR;

namespace Plataforma.Application.ViabilidadAmbiental.Commands.SolicitarViabilidadAmbiental;

// Recibe primitivos, igual que CreateLeadCommand — la construcción de Value
// Objects (Email, Telefono, DatosSolicitante, UbicacionLote) ocurre en el
// handler. PropiedadId y (Departamento/Municipio/DireccionReferencia) son
// mutuamente excluyentes — validado en el Validator y de nuevo como
// invariante del agregado (SolicitudViabilidadAmbiental.Solicitar).
public sealed record SolicitarViabilidadAmbientalCommand(
    string Nombre,
    string Email,
    string Telefono,
    string? Indicativo,
    Guid? PropiedadId,
    string? Departamento,
    string? Municipio,
    string? DireccionReferencia
) : IRequest<SolicitarViabilidadAmbientalResult>;
