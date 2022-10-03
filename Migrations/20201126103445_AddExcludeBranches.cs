using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class AddExcludeBranches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "exclude_to_branches",
                table: "settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exclude_to_branches",
                table: "settings");
        }
    }
}
