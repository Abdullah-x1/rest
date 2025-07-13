using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class _17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepName",
                table: "Forms",
                newName: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Forms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_DepartmentId",
                table: "Forms",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Department_DepartmentId",
                table: "Forms",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Department_DepartmentId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_DepartmentId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Forms");

            migrationBuilder.RenameColumn(
                name: "Departments",
                table: "Forms",
                newName: "DepName");
        }
    }
}
