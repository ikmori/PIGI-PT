using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIGI_PT_Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCategoria = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InquilinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inquilinos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreComercial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DominioRed = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PermitirIA = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquilinos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosDeHistorial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDeAccion = table.Column<int>(type: "int", nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InquilinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosDeHistorial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiesgosOperacionales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicioAfectado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescripcionAmenaza = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NivelDeImpacto = table.Column<int>(type: "int", nullable: false),
                    PlanDeMitigacion = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    FechaUltimaRevision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InquilinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiesgosOperacionales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescripcionOriginal = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    DescripcionSanitizada = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OperadorAsignadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InquilinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InquilinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_InquilinoId",
                table: "Categorias",
                column: "InquilinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_InquilinoId_NombreCategoria",
                table: "Categorias",
                columns: new[] { "InquilinoId", "NombreCategoria" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inquilinos_DominioRed",
                table: "Inquilinos",
                column: "DominioRed",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inquilinos_NombreComercial",
                table: "Inquilinos",
                column: "NombreComercial");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDeHistorial_InquilinoId",
                table: "RegistrosDeHistorial",
                column: "InquilinoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDeHistorial_TicketId",
                table: "RegistrosDeHistorial",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDeHistorial_TicketId_CreatedAt",
                table: "RegistrosDeHistorial",
                columns: new[] { "TicketId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosOperacionales_FechaUltimaRevision",
                table: "RiesgosOperacionales",
                column: "FechaUltimaRevision");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosOperacionales_InquilinoId",
                table: "RiesgosOperacionales",
                column: "InquilinoId");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosOperacionales_InquilinoId_NivelDeImpacto",
                table: "RiesgosOperacionales",
                columns: new[] { "InquilinoId", "NivelDeImpacto" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoriaId",
                table: "Tickets",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Estado",
                table: "Tickets",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_InquilinoId_CreatedAt",
                table: "Tickets",
                columns: new[] { "InquilinoId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OperadorAsignadoId",
                table: "Tickets",
                column: "OperadorAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_InquilinoId",
                table: "Usuarios",
                column: "InquilinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_InquilinoId_Email",
                table: "Usuarios",
                columns: new[] { "InquilinoId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_InquilinoId_UserName",
                table: "Usuarios",
                columns: new[] { "InquilinoId", "UserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Inquilinos");

            migrationBuilder.DropTable(
                name: "RegistrosDeHistorial");

            migrationBuilder.DropTable(
                name: "RiesgosOperacionales");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
