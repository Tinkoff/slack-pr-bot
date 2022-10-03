using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class CommentSlackMap : IEntityTypeConfiguration<CommentSlack>
    {
        public void Configure(EntityTypeBuilder<CommentSlack> builder)
        {
            builder.ToTable("comment_slack");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.SlackTs).IsRequired().HasColumnName("slack_ts");

            builder.HasOne(x => x.Comment).WithMany(x => x.SlackMessages).HasForeignKey("comment_id").OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.PullRequestChannel).WithMany().HasForeignKey("pull_request_slack_id").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
