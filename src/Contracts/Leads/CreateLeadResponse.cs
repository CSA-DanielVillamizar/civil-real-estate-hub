using Plataforma.Contracts.Common;

namespace Plataforma.Contracts.Leads;

public sealed record CreateLeadResponse(
    Guid Id,
    string Estado,
    EstimacionCostoDto? EstimacionCosto
);
