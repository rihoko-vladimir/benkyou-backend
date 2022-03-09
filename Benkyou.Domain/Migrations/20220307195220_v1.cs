using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Benkyou.Domain.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTermsAccepted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTermsAccepted",
                table: "AspNetUsers");
        }
    }
}
