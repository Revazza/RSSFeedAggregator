namespace RSSFeedAggregator.Api.Models
{
    public class ReturnFeedDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Summary { get; set; }
        public string? Link { get; set; }
    }
}
