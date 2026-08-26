using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    telefono_numero = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    telefono_indicativo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    origen = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    propiedad_de_interes_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    estimacion_minima_monto = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    estimacion_minima_moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    estimacion_maxima_monto = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    estimacion_maxima_moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    calculo_area_construccion_m2 = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    calculo_tipo_acabado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    calculo_municipio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    calculo_tipo_proyecto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    estimacion_calculada_en = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "propiedades",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_inmueble = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    precio_monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    precio_moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    municipio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    latitud = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    longitud = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    area_terreno_valor = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    area_terreno_unidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    area_construida_valor = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    area_construida_unidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    pendiente_porcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    tipo_suelo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    topografia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nivel_freatico_metros = table.Column<decimal>(type: "numeric(6,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propiedades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lead_estimacion_desglose",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    lead_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_estimacion_desglose", x => x.id);
                    table.ForeignKey(
                        name: "FK_lead_estimacion_desglose_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "propiedad_multimedia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    orden = table.Column<int>(type: "int", nullable: false),
                    propiedad_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propiedad_multimedia", x => x.id);
                    table.ForeignKey(
                        name: "FK_propiedad_multimedia_propiedades_propiedad_id",
                        column: x => x.propiedad_id,
                        principalTable: "propiedades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "propiedad_retiros_ambientales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo_fuente = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    distancia_minima_metros = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    normativa_aplicable = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    propiedad_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propiedad_retiros_ambientales", x => x.id);
                    table.ForeignKey(
                        name: "FK_propiedad_retiros_ambientales_propiedades_propiedad_id",
                        column: x => x.propiedad_id,
                        principalTable: "propiedades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lead_estimacion_desglose_lead_id",
                table: "lead_estimacion_desglose",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_leads_estado",
                table: "leads",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_leads_origen",
                table: "leads",
                column: "origen");

            migrationBuilder.CreateIndex(
                name: "IX_propiedad_multimedia_propiedad_id",
                table: "propiedad_multimedia",
                column: "propiedad_id");

            migrationBuilder.CreateIndex(
                name: "IX_propiedad_retiros_ambientales_propiedad_id",
                table: "propiedad_retiros_ambientales",
                column: "propiedad_id");

            migrationBuilder.CreateIndex(
                name: "IX_propiedades_estado",
                table: "propiedades",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_propiedades_tipo_inmueble",
                table: "propiedades",
                column: "tipo_inmueble");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lead_estimacion_desglose");

            migrationBuilder.DropTable(
                name: "propiedad_multimedia");

            migrationBuilder.DropTable(
                name: "propiedad_retiros_ambientales");

            migrationBuilder.DropTable(
                name: "leads");

            migrationBuilder.DropTable(
                name: "propiedades");
        }
    }
}
