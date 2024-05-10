using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class edit_table_lesson_confirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonConfirmations",
                table: "LessonConfirmations");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7229925f-4440-4407-89ea-dd610eba2e0e"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LessonConfirmations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonConfirmations",
                table: "LessonConfirmations",
                column: "LessonId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("5990388e-957f-48e0-9bb6-6f6b9048c289"), new DateTime(2024, 4, 23, 18, 1, 31, 956, DateTimeKind.Utc).AddTicks(4954), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonConfirmations",
                table: "LessonConfirmations");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5990388e-957f-48e0-9bb6-6f6b9048c289"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LessonConfirmations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonConfirmations",
                table: "LessonConfirmations",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("7229925f-4440-4407-89ea-dd610eba2e0e"), new DateTime(2024, 4, 23, 17, 43, 50, 266, DateTimeKind.Utc).AddTicks(41), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
