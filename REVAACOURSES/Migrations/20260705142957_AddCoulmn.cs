using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REVAACOURSES.Migrations
{
    /// <inheritdoc />
    public partial class AddCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Courses");
        }
    }
}
