using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZTACS.Server.Migrations
{
    /// <inheritdoc />
    public partial class logupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "LoginEvents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "LoginEvents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LoginEvents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "LoginEvents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LoginEvents");
        }
    }
}
