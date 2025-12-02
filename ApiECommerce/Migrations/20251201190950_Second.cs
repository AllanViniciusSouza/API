using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiECommerce.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClientesId",
                table: "Pedidos");

            migrationBuilder.AddColumn<string>(
                name: "ClienteNome",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento2",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago1",
                table: "Pedidos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago2",
                table: "Pedidos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendedorNome",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClientesId",
                table: "Pedidos",
                column: "ClientesId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClientesId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ClienteNome",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "FormaPagamento2",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ValorPago1",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ValorPago2",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "VendedorNome",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClientesId",
                table: "Pedidos",
                column: "ClientesId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
