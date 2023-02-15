using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;
using RSSFeedAggregator.Api.Db.Entities;

namespace FetcherConsole.Services
{
    public class Fetcher
    {
        private readonly DbContextOptions<RSSFeedAggregatorDbContext> _dbOptions;
        
        public Fetcher(DbContextOptions<RSSFeedAggregatorDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }
        
        
        public async Task<string> FetchAsync(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "MyUserAgent");
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error Occoured while fetching data from: {url}");
            }

            return await response.Content.ReadAsStringAsync();

        }

        private SyndicationFeed HtmlToSyndicationFeed(string strHtml)
        {
            var stringReader = new StringReader(strHtml);

            var reader = XmlReader.Create(stringReader);

            return SyndicationFeed.Load(reader);
        }


        private async Task AddNewCategoriesAsync(
            List<SyndicationCategory> syndCategories,
            RSSFeedAggregatorDbContext context)
        {
            //Categories sometimes contain dublicates

            //remove dublicates
            syndCategories = syndCategories
                .GroupBy(p => p.Name.Trim().ToLower())
                .Select(g => g.First())
                .ToList();

            var categories = context.Categories;
            var categoriesThatDontExist = syndCategories.Where(s =>
                !categories.Any(
                    c => c.Name.Trim().ToLower() == s.Name.Trim().ToLower()))
                .ToList();

            foreach (var category in categoriesThatDontExist)
            {
                var newCategory = new CategoryEntity()
                {
                    Name = category.Name.Trim().ToLower(),
                };
                await categories.AddAsync(newCategory);
            }

            await context.SaveChangesAsync();

        }

        private FeedItemEntity CreateFeedItem(SyndicationItem item)
        {
            // convert html text to normal text
            // example: <h1>Hello World</h1> -> Hello World
            var summary = item.Summary.Text;
            var parsedSummary = Regex.Replace(summary, "<[^>]*>", "").Trim();
            parsedSummary = parsedSummary.Replace("\n", " ");

            var newFeedItem = new FeedItemEntity()
            {
                Title = item.Title.Text,
                Summary = parsedSummary,
                Author = item.Authors.FirstOrDefault()?.Name ?? "Unknown",
                Link = item.Links.FirstOrDefault()?.Uri.OriginalString ?? "Unknown URL",
            };

            return newFeedItem;
        }

        private async Task AddNewFeedAsync(
            SyndicationItem item,
            List<SyndicationCategory> itemCategories,
            RSSFeedAggregatorDbContext context)
        {
            var categories = await context.Categories.ToListAsync();
            var feedCategories = categories.Where(c =>
                itemCategories.Any(ic => ic.Name.Trim().ToLower() == c.Name))
                .ToList();

            var newFeedItem = CreateFeedItem(item);
            newFeedItem.Categories = feedCategories;
            
            foreach (var category in feedCategories)
            {
                category.FeedItems.Add(newFeedItem);
            }
            await context.FeedItems.AddAsync(newFeedItem);
            await context.SaveChangesAsync();
        }

        private async Task AddNewFeedItemsAsync(
            List<SyndicationItem> items,
            RSSFeedAggregatorDbContext context)
        {
            foreach (var item in items)
            {
                var itemCategories = item.Categories.ToList();
                await AddNewCategoriesAsync(itemCategories, context);
                await AddNewFeedAsync(item, itemCategories, context);
            }
        }

        public async Task StartAsync()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await ProcessFeedRequests();

                    var sleepMs = 1000 * 60 * 1; // 1 min.
                    await Task.Delay(sleepMs);
                }
            });
            
        }

        private async Task ProcessFeedRequests()
        {
            await using var context = new RSSFeedAggregatorDbContext(_dbOptions);
            var feedRequests = context
                .FeedCheckRequests
                .AsNoTracking()
                .Include(f => f.Publisher)
                .OrderBy(f => f.CreatedAt)
                .Take(1000)
                .ToList();

            var maxActiveTasks = 10;
            var taskIndex = 0;
            var tasks = new Task[maxActiveTasks];
            for (var i = 0; i < feedRequests.Count; i++)
            {
                var feedCheckRequest = feedRequests[i];

                var task = Task.Run(async () => await ProcessFeedRequest(feedCheckRequest));
                tasks[taskIndex++] = task;

                if (taskIndex == maxActiveTasks)
                {
                    Task.WaitAll(tasks);
                    taskIndex = 0;
                    tasks = new Task[maxActiveTasks];
                }
            }
        }

        private async Task ProcessFeedRequest(FeedCheckRequest feedCheckRequest)
        {
            var url = feedCheckRequest.Publisher.Url;
            await using var db = new RSSFeedAggregatorDbContext(_dbOptions);
            Console.WriteLine("Fetching From url: " + url);

            var html = await FetchAsync(url);
            var feed = HtmlToSyndicationFeed(html);

            Console.WriteLine(url + " has items: " + feed.Items.Count());
            await AddNewFeedItemsAsync(feed.Items.ToList(), db);
            db.FeedCheckRequests.Remove(feedCheckRequest);
            await db.SaveChangesAsync();
        }
    }
}
