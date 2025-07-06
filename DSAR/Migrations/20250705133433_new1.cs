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
                name: "Attachment",
                table: "CaseStudy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "CaseStudy",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
