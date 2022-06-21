using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Api.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Token_UserCredentials_UserCredentialId",
                table: "Token");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Token",
                table: "Token");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Token");

            migrationBuilder.RenameTable(
                name: "Token",
                newName: "Tokens");

            migrationBuilder.RenameIndex(
                name: "IX_Token_UserCredentialId",
                table: "Tokens",
                newName: "IX_Tokens_UserCredentialId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserCredentialId",
                table: "Tokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid(),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_UserCredentials_UserCredentialId",
                table: "Tokens",
                column: "UserCredentialId",
                principalTable: "UserCredentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_UserCredentials_UserCredentialId",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.RenameTable(
                name: "Tokens",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_UserCredentialId",
                table: "Token",
                newName: "IX_Token_UserCredentialId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserCredentialId",
                table: "Token",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Token",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddPrimaryKey(
                name: "PK_Token",
                table: "Token",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Token_UserCredentials_UserCredentialId",
                table: "Token",
                column: "UserCredentialId",
                principalTable: "UserCredentials",
                principalColumn: "Id");
        }
    }
}
