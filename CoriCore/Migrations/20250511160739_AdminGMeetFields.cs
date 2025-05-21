using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class AdminGMeetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GMeetAccessToken",
                table: "Admins",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GMeetRefreshToken",
                table: "Admins",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GMeetTokenExpiresIn",
                table: "Admins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GMeetTokenGeneratedAt",
                table: "Admins",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GMeetAccessToken",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "GMeetRefreshToken",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "GMeetTokenExpiresIn",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "GMeetTokenGeneratedAt",
                table: "Admins");
        }
    }
}
