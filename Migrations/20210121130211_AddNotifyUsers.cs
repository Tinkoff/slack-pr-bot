using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class AddNotifyUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notify_users",
                table: "settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notify_users",
                table: "settings");
        }
    }
}
