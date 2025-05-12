using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class inittrainingroadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParallelCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoursesGroupId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TheoryPeriod",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TrainingRoadmaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MajorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRoadmaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoursesGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    TrainingRoadmapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId",
                        column: x => x.TrainingRoadmapId,
                        principalTable: "TrainingRoadmaps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingRoadmapCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingRoadmapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRoadmapCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingRoadmapCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingRoadmapCourses_TrainingRoadmaps_TrainingRoadmapId",
                        column: x => x.TrainingRoadmapId,
                        principalTable: "TrainingRoadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CoursesGroupId",
                table: "Courses",
                column: "CoursesGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroups_TrainingRoadmapId",
                table: "CoursesGroups",
                column: "TrainingRoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRoadmapCourses_CourseId",
                table: "TrainingRoadmapCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRoadmapCourses_TrainingRoadmapId",
                table: "TrainingRoadmapCourses",
                column: "TrainingRoadmapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CoursesGroups_CoursesGroupId",
                table: "Courses",
                column: "CoursesGroupId",
                principalTable: "CoursesGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CoursesGroups_CoursesGroupId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "CoursesGroups");

            migrationBuilder.DropTable(
                name: "TrainingRoadmapCourses");

            migrationBuilder.DropTable(
                name: "TrainingRoadmaps");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CoursesGroupId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CoursesGroupId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TheoryPeriod",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "PreCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParallelCourseIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
