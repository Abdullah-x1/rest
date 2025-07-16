using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DSAR.Migrations
{
    /// <inheritdoc />
    public partial class TermsAcceptedremovefrom_Form_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAccepted",
                table: "Forms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TermsAccepted",
                table: "Forms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
