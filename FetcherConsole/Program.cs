

using FetcherConsole.DataFetcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    { "https://stackoverflow.blog/feed/", 10 },
    { "https://dev.to/feed", 2 },
    //{ "https://www.freecodecamp.org/news/rss", 2000 },
    //{ "https://martinfowler.com/feed.atom", 4000 },
    //{ "https://codeblog.jonskeet.uk/feed/", 2000 },
    //{ "https://devblogs.microsoft.com/visualstudio/feed/", 2000 },
    //{ "https://feed.infoq.com/", 2000 },
    //{ "https://css-tricks.com/feed/", 2000 },
    //{ "https://codeopinion.com/feed/", 3000 },
    //{ "https://andrewlock.net/rss.xml", 2000 },
    //{ "https://michaelscodingspot.com/index.xml", 2000 },
    //{ "https://www.tabsoverspaces.com/feed.xml", 2000 },
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
        var waitTime = urlDictionary.ElementAt(i).Value;

        var task = Task.Run(async () =>
        {
            try
            {
                using var context = new RSSFeedAggregatorDbContext(optionBuilder.Options);
                context.Database.EnsureCreated();

                await fetcher.FetchAsync(context, url);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        });

        //delay between tasks
        await Task.Delay(waitTime);

    }

    await Task.Delay(10000);

    stopwatch.Stop();
    Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

    Task.WaitAll(tasks);

}



async Task Ok()
{
    
}






