using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Plataforma.Infrastructure.Reporting;

// Implementación concreta detrás de IPresupuestoPdfGenerator (Application) —
// el caso de uso no conoce QuestPDF ni ningún detalle de renderizado.
public sealed class QuestPdfPresupuestoPdfGenerator : IPresupuestoPdfGenerator
{
    public byte[] Generar(Lead lead)
    {
        var estimacion = lead.ResultadoCalculadora
            ?? throw new InvalidOperationException("No se puede generar el PDF: el lead no tiene una estimación calculada.");

        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(11));

                page.Header().Column(column =>
                {
                    column.Item().Text("Plataforma Civil e Inmobiliaria").FontSize(18).Bold();
                    column.Item().Text("Presupuesto preliminar de obra").FontSize(12).FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text("Datos de contacto").FontSize(13).Bold();
                    column.Item().Text($"Nombre: {lead.Nombre}");
                    column.Item().Text($"Correo: {lead.Email.Valor}");
                    column.Item().Text($"Teléfono: {lead.Telefono}");

                    column.Item().PaddingTop(8).Text("Detalles del proyecto").FontSize(13).Bold();
                    column.Item().Text($"Municipio: {estimacion.DatosEntrada.Municipio}");
                    column.Item().Text($"Tipo de proyecto: {estimacion.DatosEntrada.TipoProyecto}");
                    column.Item().Text($"Nivel de acabado: {estimacion.DatosEntrada.TipoAcabado}");
                    column.Item().Text($"Área de construcción: {estimacion.DatosEntrada.AreaConstruccionM2} m²");

                    column.Item().PaddingTop(8).Text("Estimado de inversión").FontSize(13).Bold();
                    column.Item()
                        .Text($"{FormatMoney(estimacion.MontoMinimo.Monto, estimacion.MontoMinimo.Moneda)} – {FormatMoney(estimacion.MontoMaximo.Monto, estimacion.MontoMaximo.Moneda)}")
                        .FontSize(16).Bold().FontColor(Colors.Green.Darken2);

                    column.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Categoría").Bold();
                            header.Cell().AlignRight().Text("Monto").Bold();
                        });

                        foreach (var item in estimacion.Desglose)
                        {
                            table.Cell().Text(item.Categoria);
                            table.Cell().AlignRight().Text(FormatMoney(item.Monto.Monto, item.Monto.Moneda));
                        }
                    });

                    column.Item().PaddingTop(15).Text(
                            "Este valor es una estimación preliminar y puede variar según el diseño final, " +
                            "especificaciones técnicas y condiciones del terreno. Un asesor te contactará con " +
                            "una cotización detallada.")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado el ").FontSize(9);
                    text.Span(estimacion.CalculadoEn.ToString("dd/MM/yyyy HH:mm")).FontSize(9).SemiBold();
                });
            });
        });

        return documento.GeneratePdf();
    }

    private static string FormatMoney(decimal monto, string moneda) => $"{monto:N0} {moneda}";
}
