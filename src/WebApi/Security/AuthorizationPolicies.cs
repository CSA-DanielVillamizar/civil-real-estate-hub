namespace Plataforma.WebApi.Security;

// Nombres de policy usados por .RequireAuthorization() en los endpoints
// administrativos — ver decisión aprobada: Admin ve todo, AsesorComercial
// queda acotado al panel de Leads.
public static class AuthorizationPolicies
{
    public const string RequiereAdmin = "RequiereAdmin";
    public const string RequiereAsesorOAdmin = "RequiereAsesorOAdmin";
}
