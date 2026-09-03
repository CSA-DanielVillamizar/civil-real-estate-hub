using MediatR;

namespace Plataforma.Application.Properties.Commands.ActualizarDatosBasicosPropiedad;

public sealed record ActualizarDatosBasicosPropiedadCommand(
    Guid PropiedadId,
    string Titulo,
    string Descripcion,
    decimal Precio,
    string Moneda
) : IRequest<ActualizarDatosBasicosPropiedadResult?>;

public sealed record ActualizarDatosBasicosPropiedadResult(Guid Id, string Titulo, string Descripcion, decimal Precio, string Moneda);
