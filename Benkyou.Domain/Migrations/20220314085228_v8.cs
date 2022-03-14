using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Benkyou.Domain.Migrations
{
    public partial class v8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_author_id",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_KanjiList_Cards_CardId",
                table: "KanjiList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Sets");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_author_id",
                table: "Sets",
                newName: "IX_Sets_author_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sets",
                table: "Sets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KanjiList_Sets_CardId",
                table: "KanjiList",
                column: "CardId",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_AspNetUsers_author_id",
                table: "Sets",
                column: "author_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KanjiList_Sets_CardId",
                table: "KanjiList");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_AspNetUsers_author_id",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sets",
                table: "Sets");

            migrationBuilder.RenameTable(
                name: "Sets",
                newName: "Cards");

            migrationBuilder.RenameIndex(
                name: "IX_Sets_author_id",
                table: "Cards",
                newName: "IX_Cards_author_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_author_id",
                table: "Cards",
                column: "author_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KanjiList_Cards_CardId",
                table: "KanjiList",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
