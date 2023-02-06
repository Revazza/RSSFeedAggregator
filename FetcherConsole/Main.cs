

using FetcherConsole.DataFetcher;
using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;


var optionBuilder = new DbContextOptionsBuilder<RSSFeedAggregatorDbContext>();
//for project's simplicity I will just hard-code sql connection string
optionBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=RSSFeedAggregator");

var urls = new List<string>()
{
    "https://stackoverflow.blog/feed/",
    "https://dev.to/feed",
    "https://www.freecodecamp.org/news/rss",
    "https://martinfowler.com/feed.atom",
    "https://codeblog.jonskeet.uk/feed/",
    "https://devblogs.microsoft.com/visualstudio/feed/",
    "https://feed.infoq.com/",
    "https://css-tricks.com/feed/",
    "https://codeopinion.com/feed/",
    "https://andrewlock.net/rss.xml",
    "https://michaelscodingspot.com/index.xml",
    "https://www.tabsoverspaces.com/feed.xml"
};



using (var context = new RSSFeedAggregatorDbContext(optionBuilder.Options))
{
    var fetcher = new Fetcher(context);

    while (true)
    {
        try
        {
            for (int i = 0; i < urls.Count; i++)
            {
                var url = urls[i];
                await fetcher.FetchAsync(url);

                Thread.Sleep(2000);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }


        Console.WriteLine();
        Thread.Sleep(30000);
    }

}








