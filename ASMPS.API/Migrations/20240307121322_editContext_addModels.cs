using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class editContext_addModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8c5e69a0-df44-404c-97b3-ac860dacde70"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeaneryId",
                table: "GroupStudents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("441ac88b-891e-4d9f-ab75-2106a36dcfcf"), new DateTime(2024, 3, 7, 12, 13, 22, 457, DateTimeKind.Utc).AddTicks(9027), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_DeaneryId",
                table: "GroupStudents",
                column: "DeaneryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupStudents_Users_DeaneryId",
                table: "GroupStudents",
                column: "DeaneryId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupStudents_Users_DeaneryId",
                table: "GroupStudents");

            migrationBuilder.DropIndex(
                name: "IX_GroupStudents_DeaneryId",
                table: "GroupStudents");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("441ac88b-891e-4d9f-ab75-2106a36dcfcf"));

            migrationBuilder.DropColumn(
                name: "DeaneryId",
                table: "GroupStudents");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("8c5e69a0-df44-404c-97b3-ac860dacde70"), new DateTime(2024, 3, 7, 11, 58, 0, 397, DateTimeKind.Utc).AddTicks(6411), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
