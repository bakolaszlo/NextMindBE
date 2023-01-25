using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextMindBE.Migrations
{
    /// <inheritdoc />
    public partial class AddedSessionIdToPing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Ping",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Ping");
        }
    }
}
