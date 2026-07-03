using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIGI_PT_Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionDeModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoriasAsignadasIds",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriasAsignadasIds",
                table: "Usuarios");
        }
    }
}
