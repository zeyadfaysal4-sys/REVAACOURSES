using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REVAACOURSES.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Instructors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
