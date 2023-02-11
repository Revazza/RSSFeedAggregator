using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;
using RSSFeedAggregator.Api.Db.Entities;

namespace FetcherConsole.Services;

public class FeedWatcher
{
    private readonly DbContextOptions<RSSFeedAggregatorDbContext> _options;

    public FeedWatcher(DbContextOptions<RSSFeedAggregatorDbContext> options)
    {
        _options = options;
    }

    public async Task StartAsync()
    {
        await Task.Run(async () =>
        {
            while (true)
            {
                await ProcessPublishers();

                var sleepMs = 1000 * 60 * 1; // 1 min.
                await Task.Delay(sleepMs);
            }
        });
    }

    private async Task ProcessPublishers()
    {
        await using var db = new RSSFeedAggregatorDbContext(_options);
        var publishers = await db.Publishers.ToListAsync();

        var maxActiveTasks = 10;
        var taskIndex = 0;
        var tasks = new Task[maxActiveTasks];
        foreach (var publisher in publishers)
        {
            var task = Task.Run(() => ProcessPublisher(publisher));

            tasks[taskIndex++] = task;

            if (taskIndex == maxActiveTasks)
            {
                Task.WaitAll(tasks);
                taskIndex = 0;
                tasks = new Task[maxActiveTasks];
            }
        }
    }

    private async Task ProcessPublisher(Publisher publisher)
    {
        var rss = await HttpUtil.FetchAsync(publisher.Url);
        var hash = StringHelper.CalculateHash(rss);

        if (publisher.ContentHash == hash)
        {
            return;
        }

        await using var db = new RSSFeedAggregatorDbContext(_options);
        db.FeedCheckRequests.Add(new FeedCheckRequest
        {
            PublisherId = publisher.Id,
            CreatedAt = DateTime.UtcNow
        });
        publisher.ContentHash = hash;
        db.Publishers.Update(publisher);
        await db.SaveChangesAsync();
        
    }
}