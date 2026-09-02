using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Leads;

public sealed record CreateLeadRequest(
    string Nombre,
    string Email,
    string Telefono,
    string? Indicativo,
    OrigenLeadDto Origen,
    Guid? PropiedadDeInteresId,
    DatosCalculoObraDto? DatosCalculoObra,
    ServicioDeInteresDto? ServicioDeInteres,
    string? Mensaje
);
