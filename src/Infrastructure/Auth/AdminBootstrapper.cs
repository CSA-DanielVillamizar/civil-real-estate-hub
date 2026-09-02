using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Infrastructure.Auth;

// Crea el primer usuario Admin al arrancar, si todavía no existe ninguno —
// reemplaza el registro público (ver decisión aprobada: "lo creo directamente
// en la base de datos"). El operador fija Bootstrap:AdminNombre/AdminEmail/
// AdminPassword como App Settings temporales él mismo (nunca a través de mí,
// ver docs/despliegue), reinicia la app una vez, y luego los retira — este
// servicio no los borra por sí mismo porque no tiene permisos sobre App
// Settings, solo sobre la base de datos.
public sealed class AdminBootstrapper : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BootstrapOptions _options;
    private readonly ILogger<AdminBootstrapper> _logger;

    public AdminBootstrapper(IServiceScopeFactory scopeFactory, IOptions<BootstrapOptions> options, ILogger<AdminBootstrapper> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.AdminNombre) ||
            string.IsNullOrWhiteSpace(_options.AdminEmail) ||
            string.IsNullOrWhiteSpace(_options.AdminPassword))
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();

        if (await usuarioRepository.ExisteAlgunoAsync(cancellationToken))
        {
            _logger.LogInformation("Bootstrap de administrador omitido: ya existe al menos un usuario.");
            return;
        }

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var usuario = Usuario.Crear(
            _options.AdminNombre,
            Email.Crear(_options.AdminEmail),
            passwordHasher.Hash(_options.AdminPassword),
            RolUsuario.Admin);

        await usuarioRepository.AddAsync(usuario, cancellationToken);
        _logger.LogWarning(
            "Usuario administrador inicial creado ({Email}). Retire las App Settings Bootstrap:AdminNombre/AdminEmail/AdminPassword ahora.",
            _options.AdminEmail);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
