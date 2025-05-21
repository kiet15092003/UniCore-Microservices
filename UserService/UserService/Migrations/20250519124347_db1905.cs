using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class db1905 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuardianId1",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Guardians",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId1",
                table: "Students",
                column: "GuardianId1");

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_PhoneNumber",
                table: "Guardians",
                column: "PhoneNumber",
                unique: true);

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

            migrationBuilder.DropIndex(
                name: "IX_Guardians_PhoneNumber",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "GuardianId1",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Guardians",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
