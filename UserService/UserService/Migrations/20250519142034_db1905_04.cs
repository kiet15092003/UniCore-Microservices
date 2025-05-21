using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class db1905_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GuardianId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GuardianId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianId1",
                table: "Students");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Guardians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_Students_StudentId",
                table: "Guardians",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_Students_StudentId",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_StudentId",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Guardians");

            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId",
                table: "Students",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId1",
                table: "Students",
                column: "GuardianId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId",
                table: "Students",
                column: "GuardianId",
                principalTable: "Guardians",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId1",
                table: "Students",
                column: "GuardianId1",
                principalTable: "Guardians",
                principalColumn: "Id");
        }
    }
}
