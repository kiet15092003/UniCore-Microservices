using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MajorService.Migrations
{
    /// <inheritdoc />
    public partial class addmajorgroupcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MajorGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "MajorGroups");
        }
    }
}
