using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dumps.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoCodeFieldsToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppliedPromoCode",
                table: "Carts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PromoDiscount",
                table: "Carts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedPromoCode",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "PromoDiscount",
                table: "Carts");
        }
    }
}
