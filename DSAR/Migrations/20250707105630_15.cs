using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class _15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Information",
                table: "Histories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Histories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Histories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_LevelId",
                table: "Histories",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_RoleId",
                table: "Histories",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_AspNetRoles_RoleId",
                table: "Histories",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Level_LevelId",
                table: "Histories",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_AspNetRoles_RoleId",
                table: "Histories");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Level_LevelId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_LevelId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_RoleId",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "Information",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Histories");
        }
    }
}
