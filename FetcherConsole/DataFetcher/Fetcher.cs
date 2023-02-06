using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RSSFeedAggregator.Api.Db;
using RSSFeedAggregator.Api.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace FetcherConsole.DataFetcher
{
    public class Fetcher
    {
        private readonly RSSFeedAggregatorDbContext _context;

        public Fetcher(RSSFeedAggregatorDbContext context)
        {
            _context = context;

        }


        private async Task AddNewFeedItemsAsync(List<FeedItemEntity> newFeedItems)
        {
            var feedItems = _context.FeedItems.ToList();

            var itemsToAdd = new List<FeedItemEntity>();

            newFeedItems.ForEach(newItem =>
            {
                bool isNewItem = true;
                foreach (var item in feedItems)
                {
                    if (newItem.Title == item.Title)
                    {
                        isNewItem = false;
                        break;
                    }
                }

                if (isNewItem)
                {
                    itemsToAdd.Add(newItem);
                }
            });

            await _context.AddRangeAsync(itemsToAdd);

            await _context.SaveChangesAsync();

            Console.WriteLine();
        }

        private List<CategoryEntity> GenerateCategories(List<SyndicationCategory> syndicationCategories)
        {
            var itemCategories = new List<CategoryEntity>();

            foreach (var category in syndicationCategories)
            {
                itemCategories.Add(new CategoryEntity { Name = category.Name });
            }

            return itemCategories;
        }

        private List<FeedItemEntity> GenerateFeedItems(SyndicationFeed feed)
        {
            var feedItems = new List<FeedItemEntity>();


            foreach (var item in feed.Items)
            {
                if (item.Title == null || item.Summary == null)
                {
                    continue;
                }
                Console.WriteLine("Item Title: {0}", item.Title.Text);
                Console.WriteLine("Item Summary: {0}", item.Summary.Text);

                var summary = item.Summary.Text.Trim();
                // convert html text to normal text
                // example: <h1>Hello World</h1> -> Hello World
                var parsedSummary = Regex.Replace(summary, "<[^>]*>", "");

                var link = item.Links.FirstOrDefault()?.Uri.OriginalString;
                var author = item.Authors.FirstOrDefault()?.Name;
                var categories = GenerateCategories(item.Categories.ToList());


                var newFeedItem = new FeedItemEntity()
                {
                    Title = item.Title?.Text,
                    Summary = parsedSummary,
                    Author = author ?? "Unknown",
                    Link = link,
                    Categories = categories
                };
                feedItems.Add(newFeedItem);

                Console.WriteLine();
            }

            return feedItems;
        }

        public async Task FetchAsync(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "MyUserAgent");
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error Occoured while fetching data from: {url}");
            }

            var html = await response.Content.ReadAsStringAsync();

            var stringReader = new StringReader(html);

            var reader = XmlReader.Create(stringReader);
            var feed = SyndicationFeed.Load(reader);

            var feedItems = GenerateFeedItems(feed);

            await AddNewFeedItemsAsync(feedItems);

        }

    }
}
