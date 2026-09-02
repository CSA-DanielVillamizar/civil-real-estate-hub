using MediatR;
using Plataforma.Application.Leads.Commands.Common;

namespace Plataforma.Application.Leads.Commands.ConvertirLead;

public sealed record ConvertirLeadCommand(Guid LeadId) : IRequest<LeadEstadoResult?>;
