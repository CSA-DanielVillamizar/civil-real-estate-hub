using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Properties;

public sealed record GetPropertiesAdminQuery(
    EstadoPropiedadDto? Estado,
    int Page = 1,
    int PageSize = 20
);
