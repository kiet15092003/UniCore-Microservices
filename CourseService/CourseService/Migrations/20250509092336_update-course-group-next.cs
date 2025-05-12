using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class updatecoursegroupnext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Handle foreign key constraints first
            // Check if the constraint exists before trying to drop it
            var sql = @"
                IF EXISTS (
                    SELECT * FROM sys.foreign_keys 
                    WHERE object_id = OBJECT_ID(N'FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId1')
                )
                BEGIN
                    ALTER TABLE [CoursesGroups] DROP CONSTRAINT [FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId1]
                END
            ";
            migrationBuilder.Sql(sql);

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId1",
                table: "CoursesGroupSemester");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId1",
                table: "CoursesGroupSemester");

            migrationBuilder.DropIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId1",
                table: "CoursesGroupSemester");

            migrationBuilder.DropIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId1",
                table: "CoursesGroupSemester");

            // Check if the index exists before dropping it
            sql = @"
                IF EXISTS (
                    SELECT * FROM sys.indexes 
                    WHERE object_id = OBJECT_ID(N'CoursesGroups')
                    AND name = N'IX_CoursesGroups_TrainingRoadmapId1'
                )
                BEGIN
                    DROP INDEX [IX_CoursesGroups_TrainingRoadmapId1] ON [CoursesGroups]
                END
            ";
            migrationBuilder.Sql(sql);

            // Drop columns that are no longer needed
            migrationBuilder.DropColumn(
                name: "CoursesGroupId1",
                table: "CoursesGroupSemester");

            migrationBuilder.DropColumn(
                name: "TrainingRoadmapId1",
                table: "CoursesGroupSemester");

            // Check if columns exist before dropping
            sql = @"
                IF COL_LENGTH('CoursesGroups', 'TrainingRoadmapId') IS NOT NULL
                BEGIN
                    ALTER TABLE [CoursesGroups] DROP COLUMN [TrainingRoadmapId]
                END
            ";
            migrationBuilder.Sql(sql);

            sql = @"
                IF COL_LENGTH('CoursesGroups', 'TrainingRoadmapId1') IS NOT NULL
                BEGIN
                    ALTER TABLE [CoursesGroups] DROP COLUMN [TrainingRoadmapId1]
                END
            ";
            migrationBuilder.Sql(sql);

            // Fix for type conversion: Create new columns with correct type, then drop old ones
            // For TrainingRoadmapId
            migrationBuilder.AddColumn<Guid>(
                name: "TrainingRoadmapId_New",
                table: "CoursesGroupSemester",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // For CoursesGroupId
            migrationBuilder.AddColumn<Guid>(
                name: "CoursesGroupId_New",
                table: "CoursesGroupSemester", 
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Drop the old columns
            migrationBuilder.DropColumn(
                name: "TrainingRoadmapId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropColumn(
                name: "CoursesGroupId",
                table: "CoursesGroupSemester");

            // Rename the new columns to the original names
            migrationBuilder.RenameColumn(
                name: "TrainingRoadmapId_New",
                table: "CoursesGroupSemester",
                newName: "TrainingRoadmapId");

            migrationBuilder.RenameColumn(
                name: "CoursesGroupId_New",
                table: "CoursesGroupSemester",
                newName: "CoursesGroupId");

            // Create the new indexes
            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId",
                table: "CoursesGroupSemester",
                column: "CoursesGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId",
                table: "CoursesGroupSemester",
                column: "TrainingRoadmapId");

            // Add the new foreign keys
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rest of the Down method remains unchanged
            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId",
                table: "CoursesGroupSemester");

            // Add back the int columns
            migrationBuilder.AddColumn<int>(
                name: "CoursesGroupId_Old",
                table: "CoursesGroupSemester",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrainingRoadmapId_Old",
                table: "CoursesGroupSemester",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Drop the Guid columns
            migrationBuilder.DropColumn(
                name: "CoursesGroupId",
                table: "CoursesGroupSemester");

            migrationBuilder.DropColumn(
                name: "TrainingRoadmapId",
                table: "CoursesGroupSemester");

            // Rename the columns back
            migrationBuilder.RenameColumn(
                name: "CoursesGroupId_Old",
                table: "CoursesGroupSemester",
                newName: "CoursesGroupId");

            migrationBuilder.RenameColumn(
                name: "TrainingRoadmapId_Old",
                table: "CoursesGroupSemester",
                newName: "TrainingRoadmapId");

            // Add back the extra columns
            migrationBuilder.AddColumn<Guid>(
                name: "CoursesGroupId1",
                table: "CoursesGroupSemester",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TrainingRoadmapId1",
                table: "CoursesGroupSemester",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "TrainingRoadmapId",
                table: "CoursesGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TrainingRoadmapId1",
                table: "CoursesGroups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_CoursesGroupId1",
                table: "CoursesGroupSemester",
                column: "CoursesGroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroupSemester_TrainingRoadmapId1",
                table: "CoursesGroupSemester",
                column: "TrainingRoadmapId1");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesGroups_TrainingRoadmapId1",
                table: "CoursesGroups",
                column: "TrainingRoadmapId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroups_TrainingRoadmaps_TrainingRoadmapId1",
                table: "CoursesGroups",
                column: "TrainingRoadmapId1",
                principalTable: "TrainingRoadmaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemester_CoursesGroups_CoursesGroupId1",
                table: "CoursesGroupSemester",
                column: "CoursesGroupId1",
                principalTable: "CoursesGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursesGroupSemester_TrainingRoadmaps_TrainingRoadmapId1",
                table: "CoursesGroupSemester",
                column: "TrainingRoadmapId1",
                principalTable: "TrainingRoadmaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
