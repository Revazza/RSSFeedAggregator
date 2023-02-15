using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;
using FetcherConsole.Services;

var optionBuilder = new DbContextOptionsBuilder<RSSFeedAggregatorDbContext>();
//for project's simplicity I will just hard-code sql connection string
optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RSSFeedAggregator");

await using var db = new RSSFeedAggregatorDbContext(optionBuilder.Options);
db.Database.EnsureCreated();

var watcher = new FeedWatcher(optionBuilder.Options);

var fetcher = new Fetcher(optionBuilder.Options);

var task1 = watcher.StartAsync();
var task2 = fetcher.StartAsync();
await Task.WhenAll(task1, task2);
