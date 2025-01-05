using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Learning.Server.Migrations
{
    /// <inheritdoc />
    public partial class adjustedPanier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PanierItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PanierItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
