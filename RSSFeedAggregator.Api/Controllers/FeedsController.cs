using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;
using RSSFeedAggregator.Api.Models;

namespace RSSFeedAggregator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        private readonly RSSFeedAggregatorDbContext _context;
        private const int MAX_FEEDS_PER_PAGE = 40;

        //for projects simplicity I won't use any design patterns
        public FeedsController(RSSFeedAggregatorDbContext context)
        {
            _context = context;
        }


        [HttpGet("get-all-feeds")]
        public async Task<IActionResult> GetAllFeeds()
        {
            var feeds = await _context.FeedItems.ToListAsync();
            return Ok(feeds);
        }

        [HttpGet("get-feed-by-category")]
        public async Task<IActionResult> GetFeedByCategory(string category)
        {
            var feeds = await _context.FeedItems
                .Include(f => f.Categories)
                .Where(fi => fi.Categories.Any(s => s.Name!.ToLower() == category.ToLower()))
                .Select(s => new ReturnFeedDto
                {
                    Id = s.Id,
                    Summary = s.Summary,
                    Author = s.Author,
                    Link = s.Link,
                    Title = s.Title,
                }
                )
                .ToListAsync();
            return Ok(feeds);
        }


        // I could mix searching by category and pagination in one method
        // but that's not important now
        [HttpGet("get-feeds-by-page")]
        public async Task<IActionResult> GetFeedsByPage(int currentPage, int feedsPerPage)
        {
            if (feedsPerPage <= 0 || feedsPerPage > MAX_FEEDS_PER_PAGE)
            {
                return BadRequest("Feeds per page must be from in interval [1,40]");
            }

            var feedsByPage = await _context.FeedItems
                .Skip(currentPage * feedsPerPage)
                .Take(feedsPerPage)
                .ToListAsync();

            return Ok(feedsByPage);
        }

    }
}
