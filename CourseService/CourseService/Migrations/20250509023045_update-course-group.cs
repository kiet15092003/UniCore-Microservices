using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class updatecoursegroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroups");

            migrationBuilder.DropIndex(
                name: "IX_CoursesGroups_TrainingRoadmapId",
                table: "CoursesGroups");

            migrationBuilder.DropColumn(
                name: "SemesterNumber",
                table: "CoursesGroups");

            migrationBuilder.DropColumn(
                name: "TrainingRoadmapId",
                table: "CoursesGroups");

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "CoursesGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CoursesGroupSemester",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    CoursesGroupId = table.Column<int>(type: "int", nullable: false),
                    CoursesGroupId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingRoadmapId = table.Column<int>(type: "int", nullable: false),
                    TrainingRoadmapId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesGroupSemester", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId1",
                        column: x => x.CoursesGroupId1,
                        principalTable: "CoursesGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId1",
                        column: x => x.TrainingRoadmapId1,
                        principalTable: "TrainingRoadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId1",
                table: "CoursesGroupSemester",
                column: "CoursesGroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId1",
                table: "CoursesGroupSemester",
                column: "TrainingRoadmapId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesGroupSemester");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "CoursesGroups");

            migrationBuilder.AddColumn<int>(
                name: "SemesterNumber",
                table: "CoursesGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TrainingRoadmapId",
                table: "CoursesGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroups_TrainingRoadmapId",
                table: "CoursesGroups",
                column: "TrainingRoadmapId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroups",
                column: "TrainingRoadmapId",
                principalTable: "TrainingRoadmaps",
                principalColumn: "Id");
        }
    }
}
