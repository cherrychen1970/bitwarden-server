using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bit.Core.Migrations.SqliteDatabase
{
    public partial class collection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId",
                table: "CollectionCipher");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CollectionUser",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CollectionCipher",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<Guid>(
                name: "CipherId1",
                table: "CollectionCipher",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionUser",
                table: "CollectionUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionCipher",
                table: "CollectionCipher",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCipher_CipherId1",
                table: "CollectionCipher",
                column: "CipherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId",
                table: "CollectionCipher",
                column: "CipherId",
                principalTable: "Cipher",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId1",
                table: "CollectionCipher",
                column: "CipherId1",
                principalTable: "Cipher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId",
                table: "CollectionCipher");

            migrationBuilder.DropForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId1",
                table: "CollectionCipher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionUser",
                table: "CollectionUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionCipher",
                table: "CollectionCipher");

            migrationBuilder.DropIndex(
                name: "IX_CollectionCipher_CipherId1",
                table: "CollectionCipher");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CollectionUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CollectionCipher");

            migrationBuilder.DropColumn(
                name: "CipherId1",
                table: "CollectionCipher");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionCipher_Cipher_CipherId",
                table: "CollectionCipher",
                column: "CipherId",
                principalTable: "Cipher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
