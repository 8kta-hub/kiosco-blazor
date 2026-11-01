using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kiosco.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VentaId",
                table: "MovimientosCaja",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SesionCajaId = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoRecibido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Vuelto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_SesionesCaja_SesionCajaId",
                        column: x => x.SesionCajaId,
                        principalTable: "SesionesCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VentaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaItems_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_VentaId",
                table: "MovimientosCaja",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaItems_ProductoId",
                table: "VentaItems",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaItems_VentaId",
                table: "VentaItems",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_SesionCajaId",
                table: "Ventas",
                column: "SesionCajaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCaja_Ventas_VentaId",
                table: "MovimientosCaja",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosCaja_Ventas_VentaId",
                table: "MovimientosCaja");

            migrationBuilder.DropTable(
                name: "VentaItems");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosCaja_VentaId",
                table: "MovimientosCaja");

            migrationBuilder.DropColumn(
                name: "VentaId",
                table: "MovimientosCaja");
        }
    }
}
