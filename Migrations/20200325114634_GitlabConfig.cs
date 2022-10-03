using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class GitlabConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_gitlab",
                table: "settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "iid",
                table: "pull_request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_gitlab",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "iid",
                table: "pull_request");
        }
    }
}
