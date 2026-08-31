namespace Plataforma.Application.Leads.Commands.GenerarPresupuestoPdf;

public sealed record GenerarPresupuestoPdfResult(
    Guid LeadId,
    string Estado,
    byte[] PdfBytes,
    string FileName
);
