using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class new1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "Cities2",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "DepartmentHeadName",
                table: "Forms");

            migrationBuilder.CreateTable(
                name: "AuthorizedContactEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovedCities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorRepresentative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorRepresentativeTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedContactEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedContactEntries_Forms_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Forms",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SnapshotAuthorizedContactEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotFormDataId = table.Column<int>(type: "int", nullable: false),
                    ApprovedCities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorRepresentative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorRepresentativeTitle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnapshotAuthorizedContactEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SnapshotAuthorizedContactEntries_SnapshotForms_SnapshotFormDataId",
                        column: x => x.SnapshotFormDataId,
                        principalTable: "SnapshotForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedContactEntries_RequestId",
                table: "AuthorizedContactEntries",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SnapshotAuthorizedContactEntries_SnapshotFormDataId",
                table: "SnapshotAuthorizedContactEntries",
                column: "SnapshotFormDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedContactEntries");

            migrationBuilder.DropTable(
                name: "SnapshotAuthorizedContactEntries");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cities2",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentHeadName",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
