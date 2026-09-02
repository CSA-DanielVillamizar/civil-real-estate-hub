using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plataforma.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarServicioDeInteresYMensajeALead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mensaje",
                table: "leads",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "servicio_de_interes",
                table: "leads",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mensaje",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "servicio_de_interes",
                table: "leads");
        }
    }
}
