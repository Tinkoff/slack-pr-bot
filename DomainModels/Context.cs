using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SlackPrBot.DomainModels.Entities;

namespace SlackPrBot.DomainModels
{
    internal class Context : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public Context()
        {
        }

        public Context(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentSlack> CommentsSlack { get; set; }
        public DbSet<PullRequest> PullRequests { get; set; }
        public DbSet<PullRequestChannel> PullRequestsChannel { get; set; }
        public DbSet<PullRequestWatch> PullRequestWatch { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=data.db");
            if (_loggerFactory != null)
            {
                optionsBuilder.EnableSensitiveDataLogging().UseLoggerFactory(_loggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}