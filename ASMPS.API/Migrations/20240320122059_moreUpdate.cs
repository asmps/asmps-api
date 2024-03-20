using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class moreUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scanner",
                table: "Scanner");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("048f1381-d85a-4cd5-a982-61e65f4909c6"));

            migrationBuilder.RenameTable(
                name: "Scanner",
                newName: "Scanners");

            migrationBuilder.RenameTable(
                name: "CodeScanner",
                newName: "CodeScanners");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scanners",
                table: "Scanners",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("22c25715-2974-4fa6-bcce-3ec0eebeb670"), new DateTime(2024, 3, 20, 12, 20, 59, 621, DateTimeKind.Utc).AddTicks(9602), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scanners",
                table: "Scanners");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22c25715-2974-4fa6-bcce-3ec0eebeb670"));

            migrationBuilder.RenameTable(
                name: "Scanners",
                newName: "Scanner");

            migrationBuilder.RenameTable(
                name: "CodeScanners",
                newName: "CodeScanner");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scanner",
                table: "Scanner",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("048f1381-d85a-4cd5-a982-61e65f4909c6"), new DateTime(2024, 3, 20, 12, 15, 51, 882, DateTimeKind.Utc).AddTicks(7361), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
