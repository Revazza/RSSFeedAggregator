

using FetcherConsole;
using FetcherConsole.DataFetcher;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using RSSFeedAggregator.Api.Db;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

var optionBuilder = new DbContextOptionsBuilder<RSSFeedAggregatorDbContext>();
//for project's simplicity I will just hard-code sql connection string
optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RSSFeedAggregator");

// we can configure wait time 
var urlDictionary = new Dictionary<string, int>
{
    { "https://stackoverflow.blog/feed/", 30 },
    { "https://dev.to/feed", 10 },
    { "https://www.freecodecamp.org/news/rss", 20 },
    { "https://martinfowler.com/feed.atom", 20 },
    { "https://codeblog.jonskeet.uk/feed/", 20 },
    { "https://devblogs.microsoft.com/visualstudio/feed/", 20 },
    { "https://feed.infoq.com/", 20 },
    { "https://css-tricks.com/feed/", 20 },
    { "https://codeopinion.com/feed/", 30 },
    { "https://andrewlock.net/rss.xml", 20 },
    { "https://michaelscodingspot.com/index.xml", 20 },
    { "https://www.tabsoverspaces.com/feed.xml", 20 },
};


var fetcher = new Fetcher();

while (true)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    Console.WriteLine("-------------------------New While loop Started-------------------------");
    Console.WriteLine();

    var tasks = new Task[urlDictionary.Count];

    for (int i = 0; i < urlDictionary.Count; i++)
    {
        var url = urlDictionary.ElementAt(i).Key;
        var delay = TimeSpan.FromSeconds(urlDictionary.ElementAt(i).Value);

        tasks[i] = Task.Run(async () =>
        {
            try
            {
                using var context = new RSSFeedAggregatorDbContext(optionBuilder.Options);
                context.Database.EnsureCreated();

                await fetcher.FetchAsync(context, delay, url);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        });


    }

    stopwatch.Stop();
    Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

    Task.WaitAll(tasks);

}



async Task Ok()
{

}






