using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZTACS.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initialalter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoginEvents",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "LoginEvents",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LoginEvents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoginEvents",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BlacklistedIps",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BlacklistedIps",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BlacklistedIps",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BlacklistedIps",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BlacklistedIps");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BlacklistedIps");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlacklistedIps");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BlacklistedIps");
        }
    }
}
