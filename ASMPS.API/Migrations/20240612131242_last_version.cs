using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class last_version : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("54043207-61df-4d82-99ca-06a8cb6dc7b5"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("8bb5cfa3-9e56-4b44-8c44-f42101235ac7"), new DateTime(2024, 6, 12, 13, 12, 41, 818, DateTimeKind.Utc).AddTicks(7094), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8bb5cfa3-9e56-4b44-8c44-f42101235ac7"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("54043207-61df-4d82-99ca-06a8cb6dc7b5"), new DateTime(2024, 4, 23, 20, 42, 11, 488, DateTimeKind.Utc).AddTicks(4790), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
