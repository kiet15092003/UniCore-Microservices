using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class removescheduleInDayIdfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicClasses_Shifts_ShiftId",
                table: "AcademicClasses");

            migrationBuilder.DropIndex(
                name: "IX_AcademicClasses_ShiftId",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "AcademicClasses");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "AcademicClasses");

            migrationBuilder.CreateTable(
                name: "ScheduleInDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleInDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleInDays_AcademicClasses_AcademicClassId",
                        column: x => x.AcademicClassId,
                        principalTable: "AcademicClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleInDays_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInDays_AcademicClassId",
                table: "ScheduleInDays",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInDays_ShiftId",
                table: "ScheduleInDays",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleInDays");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "AcademicClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "AcademicClasses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "AcademicClasses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClasses_ShiftId",
                table: "AcademicClasses",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicClasses_Shifts_ShiftId",
                table: "AcademicClasses",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
