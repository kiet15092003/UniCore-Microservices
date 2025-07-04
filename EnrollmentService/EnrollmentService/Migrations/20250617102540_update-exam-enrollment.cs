using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class updateexamenrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentExams_Enrollments_EnrollmentId",
                table: "EnrollmentExams");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentExams_Exams_ExamId",
                table: "EnrollmentExams");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "EnrollmentExams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "EnrollmentExams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentExams_Enrollments_EnrollmentId",
                table: "EnrollmentExams",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentExams_Exams_ExamId",
                table: "EnrollmentExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "EnrollmentExams",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "EnrollmentExams",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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
    }
}
