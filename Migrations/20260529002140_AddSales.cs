using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cod_scanner.Migrations
{
    /// <inheritdoc />
    public partial class AddSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerKg",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "SoldAt",
                table: "Sales",
                newName: "ScannedAt");

            migrationBuilder.AddColumn<long>(
                name: "PricePerKgCents",
                table: "Sales",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalCents",
                table: "Sales",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "PricePerKgCents",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TotalCents",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "ScannedAt",
                table: "Sales",
                newName: "SoldAt");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKg",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
