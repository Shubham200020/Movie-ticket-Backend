
using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController: ControllerBase
    {

        private readonly AppDbContext _context;
        public MovieController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }
        [HttpGet("movies-count")]
        public async Task<IActionResult> GetMoviesCount()
        {
            var count = await _context.Movies.CountAsync();
            return Ok($"Movies in DB: {count}");
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            var movie = await _context.Movies
                //.Include(m => m.Showtimes)
                //.ThenInclude(s => s.Screen)
                .FirstOrDefaultAsync(m => m.Id == id);

            return movie == null ? NotFound() : Ok(movie);
        }
        //[HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound("Movie not found");

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return Ok(movie);
        }
        [HttpGet("get-all-movies")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Movie updatedMovie)
        {
            if (id != updatedMovie.Id)
                return BadRequest("ID mismatch");

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound("Movie not found");

            // Update fields
            movie.Title = updatedMovie.Title;
            movie.Genre = updatedMovie.Genre;
            movie.Hour = updatedMovie.Hour;
            movie.Grade = updatedMovie.Grade;
            movie.BoxOffice = updatedMovie.BoxOffice;
            movie.Budget = updatedMovie.Budget;
            movie.Min=updatedMovie.Min;
         
            movie.ReleaseDate = updatedMovie.ReleaseDate;
            movie.PosterUrl = updatedMovie.PosterUrl;
            movie.WidePosterUrl = updatedMovie.WidePosterUrl;

            await _context.SaveChangesAsync();

            return Ok(movie);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound("Movie not found");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok("Movie deleted successfully");
        }



    }
}
