using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class AddFullGitlabSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "gitlab_token",
                table: "settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gitlab_url",
                table: "settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gitlab_token",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "gitlab_url",
                table: "settings");
        }
    }
}
