using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarContenidoConfianza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contenidos_confianza",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    municipio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    servicio_relacionado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    publicado = table.Column<bool>(type: "bit", nullable: false),
                    creado_en = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contenidos_confianza", x => x.id);
                });

            // Registros de muestra (gap #4, decisión aprobada por el usuario):
            // quedan SIN publicar (publicado=0) — solo visibles en el panel
            // admin, para que el equipo vea cómo luce la sección antes de
            // reemplazarlos por contenido real. Nunca deben aparecer en el
            // sitio público tal cual están escritos.
            migrationBuilder.InsertData(
                table: "contenidos_confianza",
                columns: new[] { "id", "tipo", "titulo", "descripcion", "municipio", "servicio_relacionado", "publicado", "creado_en" },
                values: new object[,]
                {
                    {
                        new Guid("9f4a1c1e-1111-4a11-8a11-000000000001"),
                        "Testimonio",
                        "Ejemplo — reemplaza con un testimonio real",
                        "Este es un testimonio de muestra para previsualizar cómo se ve la sección. Reemplázalo por la cita real de un cliente de consultoría estructural antes de publicarlo.",
                        null,
                        "ConsultoriaYDisenoEstructural",
                        false,
                        new DateTimeOffset(2026, 9, 3, 0, 0, 0, TimeSpan.Zero)
                    },
                    {
                        new Guid("9f4a1c1e-1111-4a11-8a11-000000000002"),
                        "Testimonio",
                        "Ejemplo — reemplaza con un testimonio real",
                        "Este es un testimonio de muestra para previsualizar cómo se ve la sección. Reemplázalo por la cita real de un cliente de interventoría antes de publicarlo.",
                        null,
                        "InterventoriaYPresupuestos",
                        false,
                        new DateTimeOffset(2026, 9, 3, 0, 0, 0, TimeSpan.Zero)
                    },
                    {
                        new Guid("9f4a1c1e-1111-4a11-8a11-000000000003"),
                        "Portafolio",
                        "Ejemplo de proyecto — reemplaza con un caso real",
                        "Este es un caso de portafolio de muestra para previsualizar cómo se ve la sección. Reemplázalo por un proyecto real, con su municipio y una descripción breve del alcance.",
                        "Rionegro",
                        "ConsultoriaYDisenoEstructural",
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
                name: "contenidos_confianza");
        }
    }
}
