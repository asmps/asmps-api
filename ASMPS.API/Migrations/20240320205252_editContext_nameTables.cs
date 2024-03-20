using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASMPS.API.Migrations
{
    /// <inheritdoc />
    public partial class editContext_nameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeScanners");

            migrationBuilder.DropTable(
                name: "Scanners");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("220f601e-da3f-44cc-b682-8e2b18ffdbad"));

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Campuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Audiences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PassInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false),
                    PassType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannerOwnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannerOwnerships", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("3940c95a-1ba3-45bb-bfd7-19e65c1ba4ba"), new DateTime(2024, 3, 20, 20, 52, 52, 521, DateTimeKind.Utc).AddTicks(9960), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PassInfos");

            migrationBuilder.DropTable(
                name: "ScannerOwnerships");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3940c95a-1ba3-45bb-bfd7-19e65c1ba4ba"));

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Audiences");

            migrationBuilder.CreateTable(
                name: "CodeScanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeScanners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scanners", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Discriminator", "Email", "Login", "Name", "Password", "Patronymic", "RefreshToken", "RefreshTokenExpires", "Role", "Surname" },
                values: new object[] { new Guid("220f601e-da3f-44cc-b682-8e2b18ffdbad"), new DateTime(2024, 3, 20, 12, 49, 34, 545, DateTimeKind.Utc).AddTicks(8846), "User", "pmarkelo77@gmail.com", "admin", "Павел", "admin", null, null, null, 2, "Маркелов" });
        }
    }
}
