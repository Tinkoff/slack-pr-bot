using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class ReviewerMap : IEntityTypeConfiguration<Reviewer>
    {
        public void Configure(EntityTypeBuilder<Reviewer> builder)
        {
            builder.ToTable("reviewer");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");

            builder.HasOne(x => x.User).WithMany().HasForeignKey("user_id").OnDelete(DeleteBehavior.Cascade);
        }
    }
}