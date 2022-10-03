using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels.Map
{
    internal class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique(true);
            builder.Ignore(x => x.Login);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.Email).IsRequired().HasColumnName("email");
            builder.Property(x => x.SlackUserId).HasColumnName("slack_user_id");
        }
    }
}
