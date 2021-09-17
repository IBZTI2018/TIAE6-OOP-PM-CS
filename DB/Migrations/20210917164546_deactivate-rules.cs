using Microsoft.EntityFrameworkCore.Migrations;

namespace DB.Migrations
{
    public partial class deactivaterules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "Rule",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                table: "Rule");
        }
    }
}
