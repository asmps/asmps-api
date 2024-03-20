using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class updateCodeScanner_addScanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b7f0a3bb-a52f-49d4-8a35-3a1f36c55b4f"));

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "CodeScanner",
                newName: "UserId");

            migrationBuilder.CreateTable(
                name: "Scanner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scanner", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("048f1381-d85a-4cd5-a982-61e65f4909c6"), new DateTime(2024, 3, 20, 12, 15, 51, 882, DateTimeKind.Utc).AddTicks(7361), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scanner");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("048f1381-d85a-4cd5-a982-61e65f4909c6"));

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CodeScanner",
                newName: "StudentId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("b7f0a3bb-a52f-49d4-8a35-3a1f36c55b4f"), new DateTime(2024, 3, 20, 12, 0, 17, 864, DateTimeKind.Utc).AddTicks(5991), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
