using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class updateclasssemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AcademicClasses");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Semesters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfWeeks",
                table: "Semesters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Semesters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "NumberOfWeeks",
                table: "Semesters");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Semesters");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "AcademicClasses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AcademicClasses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
