using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class removetrainingroadmapstartyear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartYear",
                table: "TrainingRoadmaps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartYear",
                table: "TrainingRoadmaps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
