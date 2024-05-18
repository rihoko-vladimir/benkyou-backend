using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class added_locks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountLocked",
                table: "UserCredentials",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountLocked",
                table: "UserCredentials");
        }
    }
}
