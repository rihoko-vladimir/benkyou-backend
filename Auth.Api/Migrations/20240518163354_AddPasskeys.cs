using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPasskeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoredCredentialIds",
                table: "UserCredentials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "PublicKeyCredentialDescriptor",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: true),
                    Transports = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicKeyCredentialDescriptor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FidoCredentials",
                columns: table => new
                {
                    AaGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DescriptorId = table.Column<byte[]>(type: "varbinary(900)", nullable: true),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserHandle = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SignatureCounter = table.Column<long>(type: "bigint", nullable: false),
                    CredType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserCredentialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidoCredentials", x => x.AaGuid);
                    table.ForeignKey(
                        name: "FK_FidoCredentials_PublicKeyCredentialDescriptor_DescriptorId",
                        column: x => x.DescriptorId,
                        principalTable: "PublicKeyCredentialDescriptor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FidoCredentials_UserCredentials_UserCredentialId",
                        column: x => x.UserCredentialId,
                        principalTable: "UserCredentials",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FidoCredentials_DescriptorId",
                table: "FidoCredentials",
                column: "DescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_FidoCredentials_UserCredentialId",
                table: "FidoCredentials",
                column: "UserCredentialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FidoCredentials");

            migrationBuilder.DropTable(
                name: "PublicKeyCredentialDescriptor");

            migrationBuilder.DropColumn(
                name: "StoredCredentialIds",
                table: "UserCredentials");
        }
    }
}
