using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class PullRequestWatchMap : IEntityTypeConfiguration<PullRequestWatch>
    {
        public void Configure(EntityTypeBuilder<PullRequestWatch> builder)
        {
            builder.ToTable("pull_request_watch");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");

            builder.HasOne(x => x.User).WithMany().HasForeignKey("user_id");
        }
    }
}
