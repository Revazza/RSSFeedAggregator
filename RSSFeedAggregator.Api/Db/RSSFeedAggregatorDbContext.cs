using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db.Entities;

namespace RSSFeedAggregator.Api.Db
{
    public class RSSFeedAggregatorDbContext : DbContext
    {
        public DbSet<FeedItemEntity> FeedItems { get; set; }



        public RSSFeedAggregatorDbContext(
            DbContextOptions<RSSFeedAggregatorDbContext> options) : base(options)
        {

        }

    }
}
