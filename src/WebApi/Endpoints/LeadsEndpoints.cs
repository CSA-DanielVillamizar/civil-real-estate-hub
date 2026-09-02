using MediatR;
using Plataforma.Contracts.Leads;
using Plataforma.WebApi.Mapping;
using Plataforma.WebApi.Security;

namespace Plataforma.WebApi.Endpoints;

public static class LeadsEndpoints
{
    public static void MapLeadsEndpoints(this WebApplication app)
    {
        app.MapPost("/api/leads", CreateAsync)
            .WithName("createLead")
            .WithTags("Leads");

        app.MapPost("/api/leads/presupuesto-pdf", GenerarPresupuestoPdfAsync)
            .WithName("generarPresupuestoPdf")
            .WithTags("Leads");

        // Panel administrativo (CRM mínimo) — accesible para Admin y para
        // AsesorComercial (rol acotado exclusivamente a Leads, ver decisión
        // aprobada de la Fase de autenticación).
        app.MapGet("/api/leads/admin", GetLeadsAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAsesorOAdmin)
            .WithName("getLeadsAdmin")
            .WithTags("Leads");

        app.MapPost("/api/leads/{id:guid}/marcar-contactado", MarcarContactadoAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAsesorOAdmin)
            .WithName("marcarLeadContactado")
            .WithTags("Leads");

        app.MapPost("/api/leads/{id:guid}/calificar", CalificarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAsesorOAdmin)
            .WithName("calificarLead")
            .WithTags("Leads");

        app.MapPost("/api/leads/{id:guid}/convertir", ConvertirAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAsesorOAdmin)
            .WithName("convertirLead")
            .WithTags("Leads");

        app.MapPost("/api/leads/{id:guid}/descartar", DescartarAsync)
            .RequireAuthorization(AuthorizationPolicies.RequiereAsesorOAdmin)
            .WithName("descartarLead")
            .WithTags("Leads");
    }

    // La validación de CreateLeadCommand ocurre en el pipeline de MediatR
    // (ValidationBehaviour, Fase 3) — cualquier fallo llega como
    // ApplicationValidationException al manejador global de errores (Prompt 6,
    // ítem 2), igual que en /api/budgets/calculate.
    private static async Task<IResult> CreateAsync(
        CreateLeadRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(), cancellationToken);
        var response = result.ToContract();

        // No existe todavía un GET /api/leads/{id} público (fuera del
        // alcance aprobado) — el Location apunta al recurso conceptual, tal
        // como autoriza el Prompt 6 ("endpoint ficticio de lectura"). El
        // panel admin usa GET /api/leads/admin para consultar leads.
        return Results.Created($"/api/leads/{response.Id}", response);
    }

    // Registra el lead (ya calificado — ver Lead.CalificarPorDescargaDePdf) y
    // devuelve el stream del PDF directamente; el renderizado vive en
    // Infrastructure (QuestPdfPresupuestoPdfGenerator) detrás de
    // IPresupuestoPdfGenerator, el endpoint solo orquesta.
    private static async Task<IResult> GenerarPresupuestoPdfAsync(
        CreateLeadRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToGenerarPresupuestoPdfCommand(), cancellationToken);

        return Results.File(result.PdfBytes, "application/pdf", result.FileName);
    }

    private static async Task<IResult> GetLeadsAsync(
        [AsParameters] GetLeadsQuery query,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query.ToApplicationQuery(), cancellationToken);
        return Results.Ok(result.Select(item => item.ToContract()));
    }

    private static async Task<IResult> MarcarContactadoAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToMarcarContactadoCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> CalificarAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToCalificarCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> ConvertirAsync(Guid id, ISender mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToConvertirCommand(), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }

    private static async Task<IResult> DescartarAsync(
        Guid id,
        DescartarLeadRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(id.ToDescartarCommand(request), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result.ToContract());
    }
}
