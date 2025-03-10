using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangePaycycleNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payCycleName",
                table: "PayCycles",
                newName: "PayCycleName");

            migrationBuilder.RenameColumn(
                name: "cycleDays",
                table: "PayCycles",
                newName: "CycleDays");

            migrationBuilder.RenameColumn(
                name: "payCycleId",
                table: "PayCycles",
                newName: "PayCycleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayCycleName",
                table: "PayCycles",
                newName: "payCycleName");

            migrationBuilder.RenameColumn(
                name: "CycleDays",
                table: "PayCycles",
                newName: "cycleDays");

            migrationBuilder.RenameColumn(
                name: "PayCycleId",
                table: "PayCycles",
                newName: "payCycleId");
        }
    }
}
