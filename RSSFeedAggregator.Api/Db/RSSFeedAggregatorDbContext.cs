using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db.Entities;

namespace RSSFeedAggregator.Api.Db
{
    public class RSSFeedAggregatorDbContext : DbContext
    {
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<FeedCheckRequest> FeedCheckRequests { get; set; }
        public DbSet<FeedItemEntity> FeedItems { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }



        public RSSFeedAggregatorDbContext(
            DbContextOptions<RSSFeedAggregatorDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryEntity>()
                .HasMany(c => c.FeedItems)
                .WithMany(f => f.Categories)
                .UsingEntity(j => j.ToTable("CategoryFeedItems"));
            
            modelBuilder.Entity<FeedCheckRequest>()
                .HasOne(c => c.Publisher)
                .WithMany()
                .HasForeignKey(c => c.PublisherId);

            modelBuilder.Entity<Publisher>().HasData(
                new() { Id = 1, ContentHash = "", Url = "https://stackoverflow.blog/feed/" },
                new() { Id = 2, ContentHash = "", Url = "https://dev.to/feed" },
                new() { Id = 3, ContentHash = "", Url = "https://www.freecodecamp.org/news/rss" },
                new() { Id = 4, ContentHash = "", Url = "https://martinfowler.com/feed.atom" },
                new() { Id = 5, ContentHash = "", Url = "https://codeblog.jonskeet.uk/feed/" },
                new() { Id = 6, ContentHash = "", Url = "https://devblogs.microsoft.com/visualstudio/feed/" },
                new() { Id = 7, ContentHash = "", Url = "https://feed.infoq.com/" },
                new() { Id = 8, ContentHash = "", Url = "https://css-tricks.com/feed/" },
                new() { Id = 9, ContentHash = "", Url = "https://codeopinion.com/feed/" },
                new() { Id = 10, ContentHash = "", Url = "https://andrewlock.net/rss.xml" },
                new() { Id = 11, ContentHash = "", Url = "https://michaelscodingspot.com/index.xml" },
                new() { Id = 12, ContentHash = "", Url = "https://www.tabsoverspaces.com/feed.xml" }
            );
        }
    }
}
