using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("movie/{movieId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMovieReviews(int movieId)
        {
            return await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .Include(r => r.User)
                .Include(r => r.Admin)
                .Select(r => new {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    r.UserId,
                    r.AdminId,
                    UserName = r.User != null ? r.User.Name : (r.Admin != null ? r.Admin.Name : "Anonymous")
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            if (review.UserId == null && review.AdminId == null)
            {
                return BadRequest("Either UserId or AdminId must be provided.");
            }

            Review? existingReview = null;
            
            if (review.UserId != null)
            {
                existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.MovieId == review.MovieId && r.UserId == review.UserId);
            }
            else if (review.AdminId != null)
            {
                existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.MovieId == review.MovieId && r.AdminId == review.AdminId);
            }

            if (existingReview != null)
            {
                // Update existing review
                existingReview.Rating = review.Rating;
                existingReview.Comment = review.Comment;
                _context.Reviews.Update(existingReview);
            }
            else
            {
                // Add new review
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            // Update movie rating (optional, but good for business logic)
            var movie = await _context.Movies.FindAsync(review.MovieId);
            if (movie != null)
            {
                var averageRating = await _context.Reviews
                    .Where(r => r.MovieId == review.MovieId)
                    .AverageAsync(r => r.Rating);
                movie.Rating = (float)Math.Round(averageRating, 1);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetMovieReviews", new { movieId = review.MovieId }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            // Update movie rating
            var movie = await _context.Movies.FindAsync(review.MovieId);
            if (movie != null)
            {
                var remainingReviews = await _context.Reviews.Where(r => r.MovieId == review.MovieId).ToListAsync();
                if (remainingReviews.Any())
                {
                    var averageRating = remainingReviews.Average(r => r.Rating);
                    movie.Rating = (float)Math.Round(averageRating, 1);
                }
                else
                {
                    movie.Rating = 0;
                }
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
