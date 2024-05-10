using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class edit_table_lesson_schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5990388e-957f-48e0-9bb6-6f6b9048c289"));

            migrationBuilder.AddColumn<bool>(
                name: "WeekType",
                table: "Schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DayId",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LessonOrderId",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("54043207-61df-4d82-99ca-06a8cb6dc7b5"), new DateTime(2024, 4, 23, 20, 42, 11, 488, DateTimeKind.Utc).AddTicks(4790), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("54043207-61df-4d82-99ca-06a8cb6dc7b5"));

            migrationBuilder.DropColumn(
                name: "WeekType",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "DayId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LessonOrderId",
                table: "Lessons");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("5990388e-957f-48e0-9bb6-6f6b9048c289"), new DateTime(2024, 4, 23, 18, 1, 31, 956, DateTimeKind.Utc).AddTicks(4954), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
