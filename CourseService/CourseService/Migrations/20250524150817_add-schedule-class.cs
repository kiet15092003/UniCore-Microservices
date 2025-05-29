using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class addscheduleclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleInDays_AcademicClassId",
                table: "ScheduleInDays");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleInDayId",
                table: "AcademicClasses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInDays_AcademicClassId",
                table: "ScheduleInDays",
                column: "AcademicClassId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleInDays_AcademicClassId",
                table: "ScheduleInDays");

            migrationBuilder.DropColumn(
                name: "ScheduleInDayId",
                table: "AcademicClasses");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInDays_AcademicClassId",
                table: "ScheduleInDays",
                column: "AcademicClassId");
        }
    }
}
