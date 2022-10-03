using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class SettingsMap : IEntityTypeConfiguration<Settings>
    {
        public void Configure(EntityTypeBuilder<Settings> builder)
        {
            builder.ToTable("settings");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.ChannelId, x.Project, x.Repo }).IsUnique(true);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.ChannelId).IsRequired().HasColumnName("channel_id");
            builder.Property(x => x.Project).IsRequired().HasColumnName("project");
            builder.Property(x => x.Repo).IsRequired().HasColumnName("repo");
            builder.Property(x => x.StaleTime).IsRequired().HasColumnName("stale_time");
            builder.Property(x => x.JiraSupport).IsRequired().HasColumnName("jira_support");
            builder.Property(x => x.DevelopmentStatuses).HasColumnName("development_statuses");
            builder.Property(x => x.ReadyToReviewStatuses).HasColumnName("ready_to_review_statuses");
            builder.Property(x => x.ReviewStatuses).HasColumnName("review_statuses");
            builder.Property(x => x.ClosedStatuses).HasColumnName("closed_statuses");
            builder.Property(x => x.ReadyStatuses).HasColumnName("ready_statuses");
            builder.Property(x => x.IsGitlab).IsRequired().HasDefaultValue(false).HasColumnName("is_gitlab");
            builder.Property(x => x.ExcludeToBranches).HasColumnName("exclude_to_branches");
            builder.Property(x => x.RepoUrl).HasColumnName("repo_url");
            builder.Property(x => x.GitlabToken).HasColumnName("gitlab_token");
            builder.Property(x => x.NotifyUsers).HasColumnName("notify_users");
        }
    }
}