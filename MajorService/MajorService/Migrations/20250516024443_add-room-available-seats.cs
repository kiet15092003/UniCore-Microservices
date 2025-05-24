﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MajorService.Migrations
{
    /// <inheritdoc />
    public partial class addroomavailableseats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Rooms");
        }
    }
}
