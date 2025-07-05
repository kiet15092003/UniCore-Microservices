using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class initclassidexam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentExam_Enrollments_EnrollmentId",
                table: "EnrollmentExam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnrollmentExam",
                table: "EnrollmentExam");

            migrationBuilder.RenameTable(
                name: "EnrollmentExam",
                newName: "EnrollmentExams");

            migrationBuilder.RenameIndex(
                name: "IX_EnrollmentExam_EnrollmentId",
                table: "EnrollmentExams",
                newName: "IX_EnrollmentExams_EnrollmentId");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamId",
                table: "EnrollmentExams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnrollmentExams",
                table: "EnrollmentExams",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Group = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ExamTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    AcademicClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentExams_ExamId",
                table: "EnrollmentExams",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentExams_Enrollments_EnrollmentId",
                table: "EnrollmentExams",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentExams_Exams_ExamId",
                table: "EnrollmentExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentExams_Enrollments_EnrollmentId",
                table: "EnrollmentExams");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentExams_Exams_ExamId",
                table: "EnrollmentExams");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EnrollmentExams",
                table: "EnrollmentExams");

            migrationBuilder.DropIndex(
                name: "IX_EnrollmentExams_ExamId",
                table: "EnrollmentExams");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "EnrollmentExams");

            migrationBuilder.RenameTable(
                name: "EnrollmentExams",
                newName: "EnrollmentExam");

            migrationBuilder.RenameIndex(
                name: "IX_EnrollmentExams_EnrollmentId",
                table: "EnrollmentExam",
                newName: "IX_EnrollmentExam_EnrollmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EnrollmentExam",
                table: "EnrollmentExam",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentExam_Enrollments_EnrollmentId",
                table: "EnrollmentExam",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id");
        }
    }
}
