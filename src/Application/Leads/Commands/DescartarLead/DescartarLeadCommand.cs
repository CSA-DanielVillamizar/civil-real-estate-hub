using MediatR;
using Plataforma.Application.Leads.Commands.Common;

namespace Plataforma.Application.Leads.Commands.DescartarLead;

public sealed record DescartarLeadCommand(Guid LeadId, string Motivo) : IRequest<LeadEstadoResult?>;
