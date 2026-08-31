using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DelmondesCodeLabs_GestaoComercial.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaleForOpenTabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerIdentifier",
                table: "Sales",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SaleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerIdentifier",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleItems");
        }
    }
}
