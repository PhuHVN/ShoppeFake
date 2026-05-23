using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeFake.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantAttributeValues_AttributeId_VariantId",
                table: "VariantAttributeValues");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "VariantAttributeValues");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AttributeValues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Attributes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributeValues_AttributeId_ProductVariantId",
                table: "VariantAttributeValues",
                columns: new[] { "AttributeId", "ProductVariantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantAttributeValues_AttributeId_ProductVariantId",
                table: "VariantAttributeValues");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AttributeValues");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attributes");

            migrationBuilder.AddColumn<int>(
                name: "VariantId",
                table: "VariantAttributeValues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributeValues_AttributeId_VariantId",
                table: "VariantAttributeValues",
                columns: new[] { "AttributeId", "VariantId" },
                unique: true);
        }
    }
}
