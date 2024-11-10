using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dumps.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVersionNumberToFloat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "VersionNumber",
                table: "ProductVersions",
                type: "real",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "VersionNumber",
                table: "ProductVersions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
