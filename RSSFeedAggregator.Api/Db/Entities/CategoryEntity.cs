using System.ComponentModel.DataAnnotations;

namespace RSSFeedAggregator.Api.Db.Entities
{
    public class CategoryEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }

        public List<FeedItemEntity> FeedItems { get; set; }

        public CategoryEntity()
        {
            FeedItems = new List<FeedItemEntity>();
        }


    }
}
