using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class db67 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OverallScore",
                table: "Enrollments",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverallScore",
                table: "Enrollments");
        }
    }
}
