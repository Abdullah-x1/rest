using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class new3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_RequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizedContactEntries_RequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.AddColumn<int>(
                name: "FormDataRequestId",
                table: "AuthorizedContactEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedContactEntries_FormDataRequestId",
                table: "AuthorizedContactEntries",
                column: "FormDataRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries",
                column: "FormDataRequestId",
                principalTable: "Forms",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.DropIndex(
                name: "IX_AuthorizedContactEntries_FormDataRequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.DropColumn(
                name: "FormDataRequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedContactEntries_RequestId",
                table: "AuthorizedContactEntries",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_RequestId",
                table: "AuthorizedContactEntries",
                column: "RequestId",
                principalTable: "Forms",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
