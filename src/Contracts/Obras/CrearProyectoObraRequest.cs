namespace Plataforma.Contracts.Obras;

public sealed record CrearProyectoObraRequest(
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    string? IndicativoCliente,
    string NombreProyecto,
    string? Descripcion,
    Guid? PropiedadId
);
