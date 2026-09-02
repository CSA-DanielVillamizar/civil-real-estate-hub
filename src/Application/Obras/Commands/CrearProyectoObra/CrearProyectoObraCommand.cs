using MediatR;

namespace Plataforma.Application.Obras.Commands.CrearProyectoObra;

public sealed record CrearProyectoObraCommand(
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    string? IndicativoCliente,
    string NombreProyecto,
    string? Descripcion,
    Guid? PropiedadId
) : IRequest<CrearProyectoObraResult>;

public sealed record CrearProyectoObraResult(Guid Id, string TokenAcceso);
