using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    employeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    dateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    jobTitle = table.Column<string>(type: "text", nullable: false),
                    department = table.Column<string>(type: "text", nullable: false),
                    salary = table.Column<decimal>(type: "numeric", nullable: false),
                    payCycleId = table.Column<int>(type: "integer", nullable: false),
                    lastPayday = table.Column<DateOnly>(type: "date", nullable: true),
                    lastPayDayIsPaid = table.Column<bool>(type: "boolean", nullable: true),
                    nextPayday = table.Column<DateOnly>(type: "date", nullable: true),
                    employType = table.Column<int>(type: "integer", nullable: false),
                    employDate = table.Column<DateOnly>(type: "date", nullable: false),
                    isSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    suspensionEndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.employeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_userId",
                table: "Employees",
                column: "userId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
