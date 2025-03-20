using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentService.Migrations
{
    /// <inheritdoc />
    public partial class fixguardianoptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId1",
                table: "Students",
                column: "GuardianId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Guardians_GuardianId1",
                table: "Students",
                column: "GuardianId1",
                principalTable: "Guardians",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Guardians_GuardianId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GuardianId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianId1",
                table: "Students");
        }
    }
}
