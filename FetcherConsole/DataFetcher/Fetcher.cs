using Azure;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using RSSFeedAggregator.Api.Db;
using RSSFeedAggregator.Api.Db.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly object obj = new object();


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

        public async Task ExecuteTaskAsync(
            DbContextOptions<RSSFeedAggregatorDbContext> dbOptions,
            KeyValuePair<string, int> dictItem)
        {

            while (true)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using var context = new RSSFeedAggregatorDbContext(dbOptions);
                context.Database.EnsureCreated();

                Console.WriteLine("Fetching From url: " + dictItem.Key);

                var html = await FetchAsync(dictItem.Key);

                var feed = HtmlToSyndicationFeed(html);

                Console.WriteLine(dictItem.Key + " has items: " + feed.Items.Count());

                await AddNewFeedItemsAsync(feed.Items.ToList(), context);

                await Task.Delay(dictItem.Value);
                stopwatch.Stop();
                Console.WriteLine($"Delay of {dictItem.Key} was seconds: {stopwatch.Elapsed.TotalSeconds}");
            }



        }


    }
}
