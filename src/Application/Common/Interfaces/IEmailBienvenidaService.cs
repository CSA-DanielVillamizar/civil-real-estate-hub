using Plataforma.Domain.Leads;

namespace Plataforma.Application.Common.Interfaces;

// "Enviar un correo transaccional de bienvenida" (Prompt Fase 2) —
// implementado en Infrastructure vía Azure Communication Services Email.
public interface IEmailBienvenidaService
{
    Task EnviarBienvenidaAsync(Lead lead, CancellationToken cancellationToken);
}
