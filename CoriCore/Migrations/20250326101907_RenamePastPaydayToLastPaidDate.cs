using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class RenamePastPaydayToLastPaidDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextPayday",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PastPaydayIsPaid",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "PastPayday",
                table: "Employees",
                newName: "LastPaidDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPaidDate",
                table: "Employees",
                newName: "PastPayday");

            migrationBuilder.AddColumn<DateOnly>(
                name: "NextPayday",
                table: "Employees",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PastPaydayIsPaid",
                table: "Employees",
                type: "boolean",
                nullable: true);
        }
    }
}
