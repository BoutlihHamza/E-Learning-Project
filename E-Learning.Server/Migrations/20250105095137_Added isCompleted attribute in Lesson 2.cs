using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Learning.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedisCompletedattributeinLesson2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "lessons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "lessons");
        }
    }
}
