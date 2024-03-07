using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class add_campus_audience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("62c6e906-079e-4bc3-ba11-5d3678a2d472"));

            migrationBuilder.AddColumn<Guid>(
                name: "AudienceId",
                table: "Lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Campus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audience_Campus_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("3b9350ff-38b3-4146-a2ac-978a74ecbcfd"), new DateTime(2024, 3, 7, 11, 27, 22, 664, DateTimeKind.Utc).AddTicks(8522), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_AudienceId",
                table: "Lessons",
                column: "AudienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_CampusId",
                table: "Audience",
                column: "CampusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Audience_AudienceId",
                table: "Lessons",
                column: "AudienceId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Audience_AudienceId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "Audience");

            migrationBuilder.DropTable(
                name: "Campus");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_AudienceId",
                table: "Lessons");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3b9350ff-38b3-4146-a2ac-978a74ecbcfd"));

            migrationBuilder.DropColumn(
                name: "AudienceId",
                table: "Lessons");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("62c6e906-079e-4bc3-ba11-5d3678a2d472"), new DateTime(2024, 3, 6, 23, 22, 14, 421, DateTimeKind.Utc).AddTicks(8174), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
