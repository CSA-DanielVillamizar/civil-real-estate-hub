using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Application.Common.Behaviours;
using Plataforma.Domain.Leads.Services;

namespace Plataforma.Application;

// Registro exclusivo de Application (MediatR, FluentValidation, pipeline de
// validación, domain services sin estado). No incluye EF Core ni infraestructura
// de persistencia — eso se registra por separado en la capa de Infraestructura (Fase 4).
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddSingleton<CalculadoraDeObraService>();

        return services;
    }
}
