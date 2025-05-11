using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class refactorcoursefields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompulsoryCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsHavePracticeClass",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ParallelCourseId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Courses",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "MinCreditCanApply",
                table: "Courses",
                newName: "MinCreditRequired");

            migrationBuilder.RenameColumn(
                name: "IsUseForCalculateScore",
                table: "Courses",
                newName: "IsRequired");

            migrationBuilder.RenameColumn(
                name: "IsOpening",
                table: "Courses",
                newName: "IsRegistrable");

            migrationBuilder.AddColumn<string>(
                name: "ParallelCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PracticePeriod",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParallelCourseIds",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PracticePeriod",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PreCourseIds",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "MinCreditRequired",
                table: "Courses",
                newName: "MinCreditCanApply");

            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "Courses",
                newName: "IsUseForCalculateScore");

            migrationBuilder.RenameColumn(
                name: "IsRegistrable",
                table: "Courses",
                newName: "IsOpening");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "Courses",
                newName: "Price");

            migrationBuilder.AddColumn<Guid>(
                name: "CompulsoryCourseId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHavePracticeClass",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParallelCourseId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
