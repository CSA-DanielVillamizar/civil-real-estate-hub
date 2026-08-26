using Plataforma.Domain.Leads;

namespace Plataforma.Application.Leads.Commands.CreateLead;

public sealed record DatosCalculoObraInput(
    decimal AreaConstruccionM2,
    TipoAcabado TipoAcabado,
    string Municipio,
    TipoProyecto TipoProyecto
);
