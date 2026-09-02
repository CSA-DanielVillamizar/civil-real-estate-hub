using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProyectosObra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "proyectos_obra",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre_cliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email_cliente = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    telefono_cliente_numero = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    telefono_cliente_indicativo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    nombre_proyecto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    propiedad_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    token_acceso = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    creado_en = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyectos_obra", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hitos_obra",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    orden = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_estimada = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_completado = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    foto_evidencia_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    proyecto_obra_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hitos_obra", x => x.id);
                    table.ForeignKey(
                        name: "FK_hitos_obra_proyectos_obra_proyecto_obra_id",
                        column: x => x.proyecto_obra_id,
                        principalTable: "proyectos_obra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hitos_obra_proyecto_obra_id",
                table: "hitos_obra",
                column: "proyecto_obra_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_obra_token_acceso",
                table: "proyectos_obra",
                column: "token_acceso",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hitos_obra");

            migrationBuilder.DropTable(
                name: "proyectos_obra");
        }
    }
}
