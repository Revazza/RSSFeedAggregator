using FetcherConsole;
using FetcherConsole.DataFetcher;
using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;
using System.Diagnostics;
using System.Resources;

var optionBuilder = new DbContextOptionsBuilder<RSSFeedAggregatorDbContext>();
//for project's simplicity I will just hard-code sql connection string
optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RSSFeedAggregator");

// we can configure wait time 
var urlDictionary = new Dictionary<string, int>
{
    { "https://stackoverflow.blog/feed/", 3000 },
    { "https://dev.to/feed", 7000 },
    { "https://www.freecodecamp.org/news/rss", 10000 },
    { "https://martinfowler.com/feed.atom", 13000 },
    { "https://codeblog.jonskeet.uk/feed/", 16000},
    { "https://devblogs.microsoft.com/visualstudio/feed/", 20000 },
    { "https://feed.infoq.com/", 24000 },
    { "https://css-tricks.com/feed/", 16000 },
    { "https://codeopinion.com/feed/", 10000 },
    { "https://andrewlock.net/rss.xml", 16000 },
    { "https://michaelscodingspot.com/index.xml", 7000 },
    { "https://www.tabsoverspaces.com/feed.xml", 4000 },

};



var fetcher = new Fetcher();


const int MAX_REQUEST_PER_TASK = 4;

try
{
    var tasks = new List<Task>();
    for (int i = 0; i < 3; i++)
    {
        for (int k = i * MAX_REQUEST_PER_TASK; k < MAX_REQUEST_PER_TASK * (i + 1) && k < urlDictionary.Count; k++)
        {
            int localK = k;
            tasks.Add(Task.Run(async () =>
            {
                await fetcher.ExecuteTaskAsync(optionBuilder.Options, urlDictionary.ElementAt(localK));
            }));
        }
    }

    await Task.WhenAll(tasks);

}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(e.InnerException);
}

//var task2 = Task.Run(async () =>
// {
//     await fetcher.ExecuteTaskAsync(optionBuilder.Options, urlDictionary.ElementAt(1));
// });


//Task.WaitAll(task1, task2);






