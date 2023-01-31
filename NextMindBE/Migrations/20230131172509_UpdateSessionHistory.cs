using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextMindBE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "UpdateInterval",
                table: "SessionHistory",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateInterval",
                table: "SessionHistory");
        }
    }
}
