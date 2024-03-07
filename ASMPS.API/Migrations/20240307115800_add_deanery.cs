using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class add_deanery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audience_Campus_CampusId",
                table: "Audience");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Audience_AudienceId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campus",
                table: "Campus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audience",
                table: "Audience");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3b9350ff-38b3-4146-a2ac-978a74ecbcfd"));

            migrationBuilder.RenameTable(
                name: "Campus",
                newName: "Campuses");

            migrationBuilder.RenameTable(
                name: "Audience",
                newName: "Audiences");

            migrationBuilder.RenameIndex(
                name: "IX_Audience_CampusId",
                table: "Audiences",
                newName: "IX_Audiences_CampusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audiences",
                table: "Audiences",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("8c5e69a0-df44-404c-97b3-ac860dacde70"), new DateTime(2024, 3, 7, 11, 58, 0, 397, DateTimeKind.Utc).AddTicks(6411), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });

            migrationBuilder.AddForeignKey(
                name: "FK_Audiences_Campuses_CampusId",
                table: "Audiences",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Audiences_AudienceId",
                table: "Lessons",
                column: "AudienceId",
                principalTable: "Audiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audiences_Campuses_CampusId",
                table: "Audiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Audiences_AudienceId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audiences",
                table: "Audiences");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8c5e69a0-df44-404c-97b3-ac860dacde70"));

            migrationBuilder.RenameTable(
                name: "Campuses",
                newName: "Campus");

            migrationBuilder.RenameTable(
                name: "Audiences",
                newName: "Audience");

            migrationBuilder.RenameIndex(
                name: "IX_Audiences_CampusId",
                table: "Audience",
                newName: "IX_Audience_CampusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campus",
                table: "Campus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audience",
                table: "Audience",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("3b9350ff-38b3-4146-a2ac-978a74ecbcfd"), new DateTime(2024, 3, 7, 11, 27, 22, 664, DateTimeKind.Utc).AddTicks(8522), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });

            migrationBuilder.AddForeignKey(
                name: "FK_Audience_Campus_CampusId",
                table: "Audience",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Audience_AudienceId",
                table: "Lessons",
                column: "AudienceId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
