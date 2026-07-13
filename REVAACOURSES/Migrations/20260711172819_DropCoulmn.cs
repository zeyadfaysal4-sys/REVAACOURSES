using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REVAACOURSES.Migrations
{
    /// <inheritdoc />
    public partial class DropCoulmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiezs_Assistants_AssistantId",
                table: "Quiezs");

            migrationBuilder.AlterColumn<int>(
                name: "AssistantId",
                table: "Quiezs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiezs_Assistants_AssistantId",
                table: "Quiezs",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiezs_Assistants_AssistantId",
                table: "Quiezs");

            migrationBuilder.AlterColumn<int>(
                name: "AssistantId",
                table: "Quiezs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiezs_Assistants_AssistantId",
                table: "Quiezs",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
