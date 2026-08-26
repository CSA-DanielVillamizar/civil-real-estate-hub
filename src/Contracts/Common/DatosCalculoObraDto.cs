namespace Plataforma.Contracts.Common;

public sealed record DatosCalculoObraDto(
    decimal AreaConstruccionM2,
    TipoAcabadoDto TipoAcabado,
    string Municipio,
    TipoProyectoDto TipoProyecto
);
