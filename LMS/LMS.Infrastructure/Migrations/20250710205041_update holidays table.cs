﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateholidaystable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeklyDaysRaw",
                table: "Holidies");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Holidies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Holidies");

            migrationBuilder.AddColumn<string>(
                name: "WeeklyDaysRaw",
                table: "Holidies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
