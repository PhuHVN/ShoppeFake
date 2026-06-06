using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeFake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgageUrl",
                table: "ProductImages",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImages",
                newName: "ImgageUrl");
        }
    }
}
