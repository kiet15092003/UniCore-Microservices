using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class addpracticetheorytypeclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChildPracticeAcademicClassIds",
                table: "AcademicClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTheoryAcademicClassId",
                table: "AcademicClasses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClasses_ParentTheoryAcademicClassId",
                table: "AcademicClasses",
                column: "ParentTheoryAcademicClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicClasses_AcademicClasses_ParentTheoryAcademicClassId",
                table: "AcademicClasses",
                column: "ParentTheoryAcademicClassId",
                principalTable: "AcademicClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicClasses_AcademicClasses_ParentTheoryAcademicClassId",
                table: "AcademicClasses");

            migrationBuilder.DropIndex(
                name: "IX_AcademicClasses_ParentTheoryAcademicClassId",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "ChildPracticeAcademicClassIds",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "ParentTheoryAcademicClassId",
                table: "AcademicClasses");
        }
    }
}
