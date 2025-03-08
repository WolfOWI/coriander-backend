using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class CapitaliseFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Users_userId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_userId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "profilePicture",
                table: "Users",
                newName: "ProfilePicture");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "googleId",
                table: "Users",
                newName: "GoogleId");

            migrationBuilder.RenameColumn(
                name: "fullName",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Employees",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "suspensionEndDate",
                table: "Employees",
                newName: "SuspensionEndDate");

            migrationBuilder.RenameColumn(
                name: "payCycleId",
                table: "Employees",
                newName: "PayCycleId");

            migrationBuilder.RenameColumn(
                name: "nextPayday",
                table: "Employees",
                newName: "NextPayday");

            migrationBuilder.RenameColumn(
                name: "lastPayday",
                table: "Employees",
                newName: "LastPayday");

            migrationBuilder.RenameColumn(
                name: "lastPayDayIsPaid",
                table: "Employees",
                newName: "LastPayDayIsPaid");

            migrationBuilder.RenameColumn(
                name: "jobTitle",
                table: "Employees",
                newName: "JobTitle");

            migrationBuilder.RenameColumn(
                name: "isSuspended",
                table: "Employees",
                newName: "IsSuspended");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Employees",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "employType",
                table: "Employees",
                newName: "EmployType");

            migrationBuilder.RenameColumn(
                name: "employDate",
                table: "Employees",
                newName: "EmployDate");

            migrationBuilder.RenameColumn(
                name: "department",
                table: "Employees",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "dateOfBirth",
                table: "Employees",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "employeeId",
                table: "Employees",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "salary",
                table: "Employees",
                newName: "SalaryAmount");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Employees",
                newName: "PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_userId",
                table: "Employees",
                newName: "IX_Employees_UserId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Admins",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "adminId",
                table: "Admins",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Admins_userId",
                table: "Admins",
                newName: "IX_Admins_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PayCycleId",
                table: "Employees",
                column: "PayCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Users_UserId",
                table: "Admins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_PayCycles_PayCycleId",
                table: "Employees",
                column: "PayCycleId",
                principalTable: "PayCycles",
                principalColumn: "payCycleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Users_UserId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_PayCycles_PayCycleId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PayCycleId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "Users",
                newName: "profilePicture");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "Users",
                newName: "googleId");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users",
                newName: "fullName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Employees",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "SuspensionEndDate",
                table: "Employees",
                newName: "suspensionEndDate");

            migrationBuilder.RenameColumn(
                name: "PayCycleId",
                table: "Employees",
                newName: "payCycleId");

            migrationBuilder.RenameColumn(
                name: "NextPayday",
                table: "Employees",
                newName: "nextPayday");

            migrationBuilder.RenameColumn(
                name: "LastPayday",
                table: "Employees",
                newName: "lastPayday");

            migrationBuilder.RenameColumn(
                name: "LastPayDayIsPaid",
                table: "Employees",
                newName: "lastPayDayIsPaid");

            migrationBuilder.RenameColumn(
                name: "JobTitle",
                table: "Employees",
                newName: "jobTitle");

            migrationBuilder.RenameColumn(
                name: "IsSuspended",
                table: "Employees",
                newName: "isSuspended");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Employees",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "EmployType",
                table: "Employees",
                newName: "employType");

            migrationBuilder.RenameColumn(
                name: "EmployDate",
                table: "Employees",
                newName: "employDate");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Employees",
                newName: "department");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Employees",
                newName: "dateOfBirth");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employees",
                newName: "employeeId");

            migrationBuilder.RenameColumn(
                name: "SalaryAmount",
                table: "Employees",
                newName: "salary");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Employees",
                newName: "phone");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                newName: "IX_Employees_userId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Admins",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Admins",
                newName: "adminId");

            migrationBuilder.RenameIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                newName: "IX_Admins_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Users_userId",
                table: "Admins",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_userId",
                table: "Employees",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
