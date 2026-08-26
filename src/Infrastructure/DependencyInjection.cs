using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Infrastructure.Persistence;
using Plataforma.Infrastructure.Persistence.Repositories;

namespace Plataforma.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "PlataformaDb";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"No se encontró la cadena de conexión '{ConnectionStringName}' en la configuración.");

        // FinOps (Fase 8): Azure SQL Database (serverless/free tier) en vez de
        // PostgreSQL — evita el costo de un servicio administrado de Postgres
        // en Azure (no existe una capa gratuita equivalente a la de Azure SQL).
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServer =>
                sqlServer.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();

        return services;
    }
}
