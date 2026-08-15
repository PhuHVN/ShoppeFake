using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeFake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCartItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConversationId",
                table: "CartItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "CartItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "CartItem");
        }
    }
}
