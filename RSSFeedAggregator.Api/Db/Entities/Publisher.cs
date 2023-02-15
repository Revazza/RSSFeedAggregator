namespace RSSFeedAggregator.Api.Db.Entities;

public class Publisher
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string ContentHash { get; set; }
}