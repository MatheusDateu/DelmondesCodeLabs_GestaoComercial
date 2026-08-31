using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DelmondesCodeLabs_GestaoComercial.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Promotions",
                newName: "DiscountValue");

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Promotions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Promotions",
                newName: "DiscountPercent");
        }
    }
}
