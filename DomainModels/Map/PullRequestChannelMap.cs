using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class PullRequestChannelMap : IEntityTypeConfiguration<PullRequestChannel>
    {
        public void Configure(EntityTypeBuilder<PullRequestChannel> builder)
        {
            builder.ToTable("pull_request_slack");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.SlackTs).IsRequired().HasColumnName("slack_ts");
            builder.Property(x => x.LastNotificationDate).HasColumnName("last_notification_date");

            builder.HasOne(x => x.Settings).WithMany().HasForeignKey("settings_id").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
