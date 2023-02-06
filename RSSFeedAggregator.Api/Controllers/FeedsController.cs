using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSSFeedAggregator.Api.Db;

namespace RSSFeedAggregator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        private readonly RSSFeedAggregatorDbContext _context;


        //for projects simplicity I won't use any pattern or repositories
        public FeedsController(RSSFeedAggregatorDbContext context)
        {
            _context = context;
        }


        [HttpGet("get-all-feeds")]
        public async Task<IActionResult> GetAllFeeds()
        {
            var feeds = await _context.FeedItems.Include(f => f.Categories).ToListAsync();
            return Ok(feeds);
        }

        [HttpGet("get-feed-by-category")]
        public async Task<IActionResult> GetFeedByCategory(string category)
        {
            var feeds = await _context.FeedItems
                .Include(f => f.Categories)
                .Where(fi => fi.Categories.Any(s => s.Name == category))
                .ToListAsync();
            return Ok(feeds);
        }


    }
}
