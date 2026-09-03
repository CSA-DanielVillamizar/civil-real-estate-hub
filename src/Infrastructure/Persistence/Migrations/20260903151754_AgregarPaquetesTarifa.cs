using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPaquetesTarifa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "paquetes_tarifa",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    servicio_relacionado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    precio_desde = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    precio_hasta = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    unidad_precio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    publicado = table.Column<bool>(type: "bit", nullable: false),
                    creado_en = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paquetes_tarifa", x => x.id);
                });

            // Registros de muestra (gap #5, decisión aprobada por el
            // usuario): quedan SIN publicar (publicado=0) — solo visibles en
            // el panel admin, para previsualizar la sección antes de
            // reemplazarlos por tarifas reales. Sin precioDesde/precioHasta
            // (null): no hay cifras reales que mostrar todavía.
            migrationBuilder.InsertData(
                table: "paquetes_tarifa",
                columns: new[] { "id", "servicio_relacionado", "titulo", "descripcion", "precio_desde", "precio_hasta", "unidad_precio", "moneda", "publicado", "creado_en" },
                values: new object[,]
                {
                    {
                        new Guid("a1b2c3d4-2222-4b22-8b22-000000000001"),
                        "ConsultoriaYDisenoEstructural",
                        "Ejemplo — reemplaza con una tarifa real",
                        "Este es un paquete de muestra para previsualizar cómo se ve la sección de precios de consultoría estructural. Reemplázalo por tu tarifa real (rango de precio y unidad) antes de publicarlo.",
                        null,
                        null,
                        "por definir",
                        "COP",
                        false,
                        new DateTimeOffset(2026, 9, 3, 0, 0, 0, TimeSpan.Zero)
                    },
                    {
                        new Guid("a1b2c3d4-2222-4b22-8b22-000000000002"),
                        "InterventoriaYPresupuestos",
                        "Ejemplo — reemplaza con una tarifa real",
                        "Este es un paquete de muestra para previsualizar cómo se ve la sección de precios de interventoría. Reemplázalo por tu tarifa real (rango de precio y unidad) antes de publicarlo.",
                        null,
                        null,
                        "por definir",
                        "COP",
                        false,
                        new DateTimeOffset(2026, 9, 3, 0, 0, 0, TimeSpan.Zero)
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // DropTable ya revierte también las filas de muestra insertadas
            // arriba — no hace falta un DeleteData explícito.
            migrationBuilder.DropTable(
                name: "paquetes_tarifa");
        }
    }
}
