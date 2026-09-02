using MediatR;
using Plataforma.Application.Leads.Commands.Common;

namespace Plataforma.Application.Leads.Commands.CalificarLead;

public sealed record CalificarLeadCommand(Guid LeadId) : IRequest<LeadEstadoResult?>;
