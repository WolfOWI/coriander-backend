using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Employees_employeeId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_EquipmentCategories_equipmentCatId",
                table: "Equipment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment");

            migrationBuilder.RenameTable(
                name: "Equipment",
                newName: "Equipments");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_equipmentCatId",
                table: "Equipments",
                newName: "IX_Equipments_equipmentCatId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipment_employeeId",
                table: "Equipments",
                newName: "IX_Equipments_employeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments",
                column: "equipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Employees_employeeId",
                table: "Equipments",
                column: "employeeId",
                principalTable: "Employees",
                principalColumn: "employeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_EquipmentCategories_equipmentCatId",
                table: "Equipments",
                column: "equipmentCatId",
                principalTable: "EquipmentCategories",
                principalColumn: "equipmentCatId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Employees_employeeId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_EquipmentCategories_equipmentCatId",
                table: "Equipments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipments",
                table: "Equipments");

            migrationBuilder.RenameTable(
                name: "Equipments",
                newName: "Equipment");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_equipmentCatId",
                table: "Equipment",
                newName: "IX_Equipment_equipmentCatId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_employeeId",
                table: "Equipment",
                newName: "IX_Equipment_employeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipment",
                table: "Equipment",
                column: "equipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Employees_employeeId",
                table: "Equipment",
                column: "employeeId",
                principalTable: "Employees",
                principalColumn: "employeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_EquipmentCategories_equipmentCatId",
                table: "Equipment",
                column: "equipmentCatId",
                principalTable: "EquipmentCategories",
                principalColumn: "equipmentCatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
