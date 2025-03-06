using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoriCore.Migrations
{
    /// <inheritdoc />
    public partial class CamelCaseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.RenameColumn(
                name: "profile_picture",
                table: "Users",
                newName: "profilePicture");

            migrationBuilder.RenameColumn(
                name: "google_id",
                table: "Users",
                newName: "googleId");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Users",
                newName: "fullName");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "profilePicture",
                table: "Users",
                newName: "profile_picture");

            migrationBuilder.RenameColumn(
                name: "googleId",
                table: "Users",
                newName: "google_id");

            migrationBuilder.RenameColumn(
                name: "fullName",
                table: "Users",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "user_id");

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_Admin_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_user_id",
                table: "Admin",
                column: "user_id",
                unique: true);
        }
    }
}
