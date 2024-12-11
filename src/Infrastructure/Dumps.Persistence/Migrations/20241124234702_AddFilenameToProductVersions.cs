using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dumps.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFilenameToProductVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProductVersions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProductVersions");
        }
    }
}
