using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dumps.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCurrentVersionNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductVersions_CurrentVersionId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CurrentVersionId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                table: "Products",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentVersionId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CurrentVersionId",
                table: "Products",
                column: "CurrentVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductVersions_CurrentVersionId",
                table: "Products",
                column: "CurrentVersionId",
                principalTable: "ProductVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
