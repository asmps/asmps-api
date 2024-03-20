using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class editCodeScanner_haskey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22c25715-2974-4fa6-bcce-3ec0eebeb670"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CodeScanners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CodeScanners",
                table: "CodeScanners",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("220f601e-da3f-44cc-b682-8e2b18ffdbad"), new DateTime(2024, 3, 20, 12, 49, 34, 545, DateTimeKind.Utc).AddTicks(8846), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CodeScanners",
                table: "CodeScanners");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("220f601e-da3f-44cc-b682-8e2b18ffdbad"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CodeScanners");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("22c25715-2974-4fa6-bcce-3ec0eebeb670"), new DateTime(2024, 3, 20, 12, 20, 59, 621, DateTimeKind.Utc).AddTicks(9602), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
