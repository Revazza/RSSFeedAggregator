namespace RSSFeedAggregator.Api.Db.Entities;

public class FeedCheckRequest
{
    public int Id { get; set; }
    public int PublisherId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Publisher Publisher { get; set; }
}