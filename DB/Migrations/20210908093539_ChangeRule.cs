using Microsoft.EntityFrameworkCore.Migrations;

namespace DB.Migrations
{
    public partial class ChangeRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "Rule");

            migrationBuilder.AddColumn<string>(
                name: "condition",
                table: "Rule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "parentId",
                table: "Rule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transformation",
                table: "Rule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rule_parentId",
                table: "Rule",
                column: "parentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rule_Rule_parentId",
                table: "Rule",
                column: "parentId",
                principalTable: "Rule",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rule_Rule_parentId",
                table: "Rule");

            migrationBuilder.DropIndex(
                name: "IX_Rule_parentId",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "condition",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "parentId",
                table: "Rule");

            migrationBuilder.DropColumn(
                name: "transformation",
                table: "Rule");

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "Rule",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
