using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class db1806 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentResults_ScoreTypes_ScoreTypeId",
                table: "StudentResults");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScoreTypeId",
                table: "StudentResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResults_ScoreTypes_ScoreTypeId",
                table: "StudentResults",
                column: "ScoreTypeId",
                principalTable: "ScoreTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentResults_ScoreTypes_ScoreTypeId",
                table: "StudentResults");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScoreTypeId",
                table: "StudentResults",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResults_ScoreTypes_ScoreTypeId",
                table: "StudentResults",
                column: "ScoreTypeId",
                principalTable: "ScoreTypes",
                principalColumn: "Id");
        }
    }
}
