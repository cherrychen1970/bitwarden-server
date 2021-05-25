using Microsoft.EntityFrameworkCore.Migrations;

namespace Bit.Core.Migrations
{
    public partial class collectionChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadAccess",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "WriteAccess",
                table: "Collection");

            migrationBuilder.AddColumn<bool>(
                name: "AdminOnly",
                table: "Collection",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "Collection",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminOnly",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "ReadOnly",
                table: "Collection");

            migrationBuilder.AddColumn<byte>(
                name: "ReadAccess",
                table: "Collection",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "WriteAccess",
                table: "Collection",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
