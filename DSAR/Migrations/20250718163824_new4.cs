using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class new4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.AlterColumn<int>(
                name: "FormDataRequestId",
                table: "AuthorizedContactEntries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries",
                column: "FormDataRequestId",
                principalTable: "Forms",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries");

            migrationBuilder.AlterColumn<int>(
                name: "FormDataRequestId",
                table: "AuthorizedContactEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizedContactEntries_Forms_FormDataRequestId",
                table: "AuthorizedContactEntries",
                column: "FormDataRequestId",
                principalTable: "Forms",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
