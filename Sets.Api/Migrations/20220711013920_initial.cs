using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sets.Api.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: false),
                    author_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kanji",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiChar = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kanji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kanji_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kunyomis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunyomis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kunyomis_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Onyomis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KanjiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Onyomis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Onyomis_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kanji_SetId",
                table: "Kanji",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_Kunyomis_KanjiId",
                table: "Kunyomis",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_Onyomis_KanjiId",
                table: "Onyomis",
                column: "KanjiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kunyomis");

            migrationBuilder.DropTable(
                name: "Onyomis");

            migrationBuilder.DropTable(
                name: "Kanji");

            migrationBuilder.DropTable(
                name: "Sets");
        }
    }
}
