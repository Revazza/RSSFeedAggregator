using Microsoft.EntityFrameworkCore;

namespace RSSFeedAggregator.Api.Db
{
    public class RSSFeedAggregatorDbContext : DbContext
    {

        public RSSFeedAggregatorDbContext(
            DbContextOptions<RSSFeedAggregatorDbContext> options) : base(options)
        {

        }
    }
}
