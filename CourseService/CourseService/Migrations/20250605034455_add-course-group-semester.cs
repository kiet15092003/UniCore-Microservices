using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class addcoursegroupsemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesGroupSemester",
                table: "CoursesGroupSemester");

            migrationBuilder.RenameTable(
                name: "CoursesGroupSemester",
                newName: "CoursesGroupSemesters");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId",
                table: "CoursesGroupSemesters",
                newName: "IX_CoursesGroupSemesters_TrainingRoadmapId");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId",
                table: "CoursesGroupSemesters",
                newName: "IX_CoursesGroupSemesters_CoursesGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesGroupSemesters",
                table: "CoursesGroupSemesters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemesters_CoursesGroups_CoursesGroupId",
                table: "CoursesGroupSemesters",
                column: "CoursesGroupId",
                principalTable: "CoursesGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemesters_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroupSemesters",
                column: "TrainingRoadmapId",
                principalTable: "TrainingRoadmaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemesters_CoursesGroups_CoursesGroupId",
                table: "CoursesGroupSemesters");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemesters_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroupSemesters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesGroupSemesters",
                table: "CoursesGroupSemesters");

            migrationBuilder.RenameTable(
                name: "CoursesGroupSemesters",
                newName: "CoursesGroupSemester");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesGroupSemesters_TrainingRoadmapId",
                table: "CoursesGroupSemester",
                newName: "IX_CoursesGroupSemester_TrainingRoadmapId");

            migrationBuilder.RenameIndex(
                name: "IX_CoursesGroupSemesters_CoursesGroupId",
                table: "CoursesGroupSemester",
                newName: "IX_CoursesGroupSemester_CoursesGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesGroupSemester",
                table: "CoursesGroupSemester",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId",
                table: "CoursesGroupSemester",
                column: "CoursesGroupId",
                principalTable: "CoursesGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroupSemester",
                column: "TrainingRoadmapId",
                principalTable: "TrainingRoadmaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
