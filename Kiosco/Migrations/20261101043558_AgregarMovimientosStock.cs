using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kiosco.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMovimientosStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MovimientosStock",
                columns: new[] { "Id", "Cantidad", "Fecha", "Motivo", "Observacion", "ProductoId", "Tipo" },
                values: new object[,]
                {
                    { 1, 24, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 1, "Entrada" },
                    { 2, 4, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 2, "Entrada" },
                    { 3, 18, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 3, "Entrada" },
                    { 4, 2, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 4, "Entrada" },
                    { 5, 1, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 5, "Entrada" },
                    { 6, 40, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 6, "Entrada" },
                    { 7, 15, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 7, "Entrada" },
                    { 8, 10, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 8, "Entrada" },
                    { 9, 20, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 9, "Entrada" },
                    { 10, 8, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Ajuste", "Stock inicial", 10, "Entrada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductoId_Fecha",
                table: "MovimientosStock",
                columns: new[] { "ProductoId", "Fecha" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosStock");
        }
    }
}
