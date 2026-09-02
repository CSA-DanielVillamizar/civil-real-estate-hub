using Plataforma.Domain.Obras;

namespace Plataforma.Application.Obras.Queries.Common;

internal static class ProyectoObraMapping
{
    public static ProyectoObraDetalle ToDetalle(this ProyectoObra p) => new(
        p.Id.Value,
        p.NombreCliente,
        p.EmailCliente.Valor,
        p.TelefonoCliente.ToString(),
        p.NombreProyecto,
        p.Descripcion,
        p.PropiedadId?.Value,
        p.TokenAcceso,
        p.Estado,
        p.CreadoEn,
        p.Hitos
            .OrderBy(h => h.Orden)
            .Select(h => new HitoItem(h.Id, h.Nombre, h.Descripcion, h.Orden, h.Estado, h.FechaEstimada, h.FechaCompletado, h.FotoEvidenciaUrl))
            .ToList());
}
