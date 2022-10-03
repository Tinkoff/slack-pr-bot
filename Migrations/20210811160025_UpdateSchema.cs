using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_settings_channel_id_stash_project_stash_repo",
                table: "settings");

            migrationBuilder.RenameColumn(
                name: "gitlab_url",
                table: "settings",
                newName: "repo_url");

            migrationBuilder.RenameColumn(
                name: "stash_project",
                table: "settings",
                newName: "project");

            migrationBuilder.RenameColumn(
                name: "stash_repo",
                table: "settings",
                newName: "repo");

            migrationBuilder.RenameColumn(
                name: "stash_stale_time",
                table: "settings",
                newName: "stale_time");


            migrationBuilder.CreateIndex(
                name: "IX_settings_channel_id_project_repo",
                table: "settings",
                columns: new[] { "channel_id", "project", "repo" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_settings_channel_id_project_repo",
                table: "settings");

            migrationBuilder.RenameColumn(
               name: "repo_url",
               table: "settings",
               newName: "gitlab_url");

            migrationBuilder.RenameColumn(
                name: "project",
                table: "settings",
                newName: "stash_project");

            migrationBuilder.RenameColumn(
                name: "repo",
                table: "settings",
                newName: "stash_repo");

            migrationBuilder.RenameColumn(
                name: "stale_time",
                table: "settings",
                newName: "stash_stale_time");

            migrationBuilder.CreateIndex(
                name: "IX_settings_channel_id_stash_project_stash_repo",
                table: "settings",
                columns: new[] { "channel_id", "stash_project", "stash_repo" },
                unique: true);
        }
    }
}
