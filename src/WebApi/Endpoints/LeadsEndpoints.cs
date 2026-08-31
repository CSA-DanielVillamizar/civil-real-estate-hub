using MediatR;
using Plataforma.Contracts.Leads;
using Plataforma.WebApi.Mapping;

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

        // No existe todavía un GET /api/leads/{id} (fuera del alcance de la
        // Fase 2 aprobada) — el Location apunta al recurso conceptual, tal
        // como autoriza el Prompt 6 ("endpoint ficticio de lectura").
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
}
