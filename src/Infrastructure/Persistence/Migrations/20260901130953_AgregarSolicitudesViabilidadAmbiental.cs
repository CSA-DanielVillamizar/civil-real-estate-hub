using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSolicitudesViabilidadAmbiental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "solicitudes_viabilidad_ambiental",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    solicitante_nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    solicitante_email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    solicitante_telefono_numero = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    solicitante_telefono_indicativo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    propiedad_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    lote_departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    lote_municipio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    lote_direccion_referencia = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    pago_confirmado_en = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitudes_viabilidad_ambiental", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_solicitudes_viabilidad_ambiental_estado",
                table: "solicitudes_viabilidad_ambiental",
                column: "estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "solicitudes_viabilidad_ambiental");
        }
    }
}
