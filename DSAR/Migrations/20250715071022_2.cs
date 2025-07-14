using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Depend",
                table: "Forms");

            migrationBuilder.RenameColumn(
                name: "Field6",
                table: "Forms",
                newName: "ServiceTypeAndLocation");

            migrationBuilder.RenameColumn(
                name: "Field5",
                table: "Forms",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "Field4",
                table: "Forms",
                newName: "ServiceDescription");

            migrationBuilder.RenameColumn(
                name: "Field3",
                table: "Forms",
                newName: "ProcedureNumber");

            migrationBuilder.RenameColumn(
                name: "Field2",
                table: "Forms",
                newName: "HasDependency");

            migrationBuilder.RenameColumn(
                name: "Field1",
                table: "Forms",
                newName: "DependencyDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceTypeAndLocation",
                table: "Forms",
                newName: "Field6");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "Forms",
                newName: "Field5");

            migrationBuilder.RenameColumn(
                name: "ServiceDescription",
                table: "Forms",
                newName: "Field4");

            migrationBuilder.RenameColumn(
                name: "ProcedureNumber",
                table: "Forms",
                newName: "Field3");

            migrationBuilder.RenameColumn(
                name: "HasDependency",
                table: "Forms",
                newName: "Field2");

            migrationBuilder.RenameColumn(
                name: "DependencyDetails",
                table: "Forms",
                newName: "Field1");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Forms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Depend",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
