using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db.Entities;

namespace RSSFeedAggregator.Api.Db
{
    public class RSSFeedAggregatorDbContext : DbContext
    {
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

        }
    }
}
