using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    channel_id = table.Column<string>(nullable: false),
                    development_statuses = table.Column<string>(nullable: true),
                    jira_support = table.Column<bool>(nullable: false),
                    ready_statuses = table.Column<string>(nullable: true),
                    ready_to_review_statuses = table.Column<string>(nullable: true),
                    review_statuses = table.Column<string>(nullable: true),
                    stash_project = table.Column<string>(nullable: false),
                    stash_repo = table.Column<string>(nullable: false),
                    stash_stale_time = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    slack_user_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pull_request",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    author_id = table.Column<string>(nullable: true),
                    created = table.Column<DateTime>(nullable: false),
                    from_ref = table.Column<string>(nullable: false),
                    last_active_date = table.Column<DateTime>(nullable: false),
                    project_name = table.Column<string>(nullable: false),
                    pull_id = table.Column<int>(nullable: false),
                    repo_name = table.Column<string>(nullable: false),
                    task_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pull_request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pull_request_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    author_id = table.Column<string>(nullable: true),
                    comment_id = table.Column<int>(nullable: false),
                    pull_request_id = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comment_pull_request_pull_request_id",
                        column: x => x.pull_request_id,
                        principalTable: "pull_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pull_request_slack",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    last_notification_date = table.Column<DateTime>(nullable: true),
                    pull_request_id = table.Column<string>(nullable: true),
                    settings_id = table.Column<string>(nullable: true),
                    slack_ts = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pull_request_slack", x => x.id);
                    table.ForeignKey(
                        name: "FK_pull_request_slack_pull_request_pull_request_id",
                        column: x => x.pull_request_id,
                        principalTable: "pull_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pull_request_slack_settings_settings_id",
                        column: x => x.settings_id,
                        principalTable: "settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pull_request_watch",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    pull_request_id = table.Column<string>(nullable: true),
                    user_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pull_request_watch", x => x.id);
                    table.ForeignKey(
                        name: "FK_pull_request_watch_pull_request_pull_request_id",
                        column: x => x.pull_request_id,
                        principalTable: "pull_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pull_request_watch_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reviewer",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    pull_request_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    user_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviewer", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviewer_pull_request_pull_request_id",
                        column: x => x.pull_request_id,
                        principalTable: "pull_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviewer_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_slack",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    comment_id = table.Column<string>(nullable: true),
                    pull_request_slack_id = table.Column<string>(nullable: true),
                    slack_ts = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_slack", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_slack_comment_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_slack_pull_request_slack_pull_request_slack_id",
                        column: x => x.pull_request_slack_id,
                        principalTable: "pull_request_slack",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comment_author_id",
                table: "comment",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_pull_request_id",
                table: "comment",
                column: "pull_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_slack_comment_id",
                table: "comment_slack",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_slack_pull_request_slack_id",
                table: "comment_slack",
                column: "pull_request_slack_id");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_from_ref",
                table: "pull_request",
                column: "from_ref");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_author_id",
                table: "pull_request",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_slack_pull_request_id",
                table: "pull_request_slack",
                column: "pull_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_slack_settings_id",
                table: "pull_request_slack",
                column: "settings_id");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_watch_pull_request_id",
                table: "pull_request_watch",
                column: "pull_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_pull_request_watch_user_id",
                table: "pull_request_watch",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviewer_pull_request_id",
                table: "reviewer",
                column: "pull_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviewer_user_id",
                table: "reviewer",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_settings_channel_id_stash_project_stash_repo",
                table: "settings",
                columns: new[] { "channel_id", "stash_project", "stash_repo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_slack");

            migrationBuilder.DropTable(
                name: "pull_request_watch");

            migrationBuilder.DropTable(
                name: "reviewer");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "pull_request_slack");

            migrationBuilder.DropTable(
                name: "pull_request");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
