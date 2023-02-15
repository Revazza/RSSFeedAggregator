using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSSFeedAggregator.Api.Db.Entities
{
    public class FeedItemEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int PublisherId { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Summary { get; set; }
        public string? Link { get; set; }

        public List<CategoryEntity> Categories { get; set; }

        public FeedItemEntity()
        {
            Categories = new List<CategoryEntity>();
        }



    }
}
