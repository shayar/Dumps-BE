using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dumps.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserchangesremovingmiddlename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
