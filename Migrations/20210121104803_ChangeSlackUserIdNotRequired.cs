using Microsoft.EntityFrameworkCore.Migrations;

namespace SlackPrBot.Migrations
{
    public partial class ChangeSlackUserIdNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA writable_schema = 1;" +
                                "UPDATE SQLITE_MASTER SET SQL = 'CREATE TABLE \"user\" (" +
                                "\"id\" TEXT NOT NULL CONSTRAINT \"PK_user\" PRIMARY KEY," +
                                    "\"email\" TEXT NOT NULL," +
                                    "\"slack_user_id\" TEXT NULL" +
                                ")' WHERE NAME = 'user';" +
                                "PRAGMA writable_schema = 0;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA writable_schema = 1;" +
                               "UPDATE SQLITE_MASTER SET SQL = 'CREATE TABLE \"user\" (" +
                               "\"id\" TEXT NOT NULL CONSTRAINT \"PK_user\" PRIMARY KEY," +
                                   "\"email\" TEXT NOT NULL," +
                                   "\"slack_user_id\" TEXT NOT NULL" +
                               ")' WHERE NAME = 'user';" +
                               "PRAGMA writable_schema = 0;");
        }
    }
}
