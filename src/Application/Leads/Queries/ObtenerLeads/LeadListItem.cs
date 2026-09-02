using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Queries.ObtenerLeads;

public sealed record LeadListItem(
    Guid Id,
    string Nombre,
    string Email,
    string Telefono,
    OrigenLead Origen,
    EstadoLead Estado,
    DateTimeOffset CapturadoEn,
    Guid? PropiedadDeInteresId,
    decimal? EstimacionMontoMinimo,
    decimal? EstimacionMontoMaximo,
    string? EstimacionMoneda,
    ServicioDeInteres? ServicioDeInteres,
    string? Mensaje);
