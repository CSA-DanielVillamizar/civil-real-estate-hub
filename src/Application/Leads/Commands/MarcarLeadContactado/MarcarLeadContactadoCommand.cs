using MediatR;
using Plataforma.Application.Leads.Commands.Common;

namespace Plataforma.Application.Leads.Commands.MarcarLeadContactado;

public sealed record MarcarLeadContactadoCommand(Guid LeadId) : IRequest<LeadEstadoResult?>;
