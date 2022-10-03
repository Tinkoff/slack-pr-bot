using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class PullRequestMap : IEntityTypeConfiguration<PullRequest>
    {
        public void Configure(EntityTypeBuilder<PullRequest> builder)
        {
            builder.ToTable("pull_request");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.FromRef);

            builder.Property(x => x.PullId).IsRequired().HasColumnName("pull_id");
            builder.Property(x => x.FromRef).IsRequired().HasColumnName("from_ref");
            builder.Property(x => x.Created).IsRequired().HasColumnName("created");
            builder.Property(x => x.LastActiveDate).IsRequired().HasColumnName("last_active_date");
            builder.Property(x => x.TaskId).IsRequired().HasColumnName("task_id");
            builder.Property(x => x.ProjectName).IsRequired().HasColumnName("project_name");
            builder.Property(x => x.RepoName).IsRequired().HasColumnName("repo_name");
            builder.Property(x => x.Iid).HasColumnName("iid");

            builder.HasOne(x => x.Author).WithMany().HasForeignKey("author_id");

            builder.HasMany(x => x.Comments).WithOne(x => x.PullRequest).HasForeignKey("pull_request_id").OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Watch).WithOne(x => x.PullRequest).HasForeignKey("pull_request_id").OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.ChannelMap).WithOne(x => x.PullRequest).HasForeignKey("pull_request_id").OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Reviewers).WithOne(x => x.PullRequest).HasForeignKey("pull_request_id").OnDelete(DeleteBehavior.Cascade);
        }
    }
}