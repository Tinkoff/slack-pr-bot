using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class CommentMap : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comment");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.CommentId).IsRequired().HasColumnName("comment_id");
            builder.Property(x => x.Text).IsRequired().HasColumnName("text");

            builder.HasOne(x => x.Author).WithMany().HasForeignKey("author_id");
        }
    }
}
