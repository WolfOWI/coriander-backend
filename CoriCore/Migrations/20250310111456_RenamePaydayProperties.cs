using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class RenamePaydayProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPayday",
                table: "Employees",
                newName: "PastPayday");

            migrationBuilder.RenameColumn(
                name: "LastPayDayIsPaid",
                table: "Employees",
                newName: "PastPaydayIsPaid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PastPaydayIsPaid",
                table: "Employees",
                newName: "LastPayDayIsPaid");

            migrationBuilder.RenameColumn(
                name: "PastPayday",
                table: "Employees",
                newName: "LastPayday");
        }
    }
}
