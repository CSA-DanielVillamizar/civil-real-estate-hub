using MediatR;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Properties.Commands.AgregarMultimediaAPropiedad;

// El Stream (contenido del archivo) viaja hasta acá desde el endpoint —
// mismo criterio que GenerarPresupuestoPdfCommandHandler orquestando
// IPresupuestoPdfGenerator: la subida a Blob Storage se coordina desde el
// handler, no desde el endpoint, para mantener la orquestación en
// Application.
public sealed record AgregarMultimediaAPropiedadCommand(
    Guid PropiedadId,
    Stream Contenido,
    string NombreArchivo,
    string ContentType,
    TipoMultimedia Tipo
) : IRequest<AgregarMultimediaAPropiedadResult?>;
