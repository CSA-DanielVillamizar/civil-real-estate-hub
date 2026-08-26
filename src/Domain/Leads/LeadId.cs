namespace Plataforma.Domain.Leads;

public readonly record struct LeadId(Guid Value)
{
    public static LeadId Nueva() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
