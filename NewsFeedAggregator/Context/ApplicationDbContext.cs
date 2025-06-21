using Microsoft.EntityFrameworkCore;
using NewsFeedAggregator.Models;

namespace NewsFeedAggregator.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<NewsArticles> NewsArticles { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        // Tables (we’ll add these soon)
        // public DbSet<User> Users { get; set; }
        // public DbSet<NewsArticle> NewsArticles { get; set; }
        // public DbSet<UserPreference> UserPreferences { get; set; }
    }

}
