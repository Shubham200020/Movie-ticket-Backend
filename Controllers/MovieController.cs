
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
        [HttpGet("recomened")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetRecommendedMovies()
        {
            var movies = await _context.Movies
                .Where(m => m.Recomended == true)
                .ToListAsync();

            return Ok(movies);
        }
        [HttpGet("movies-count")]
        public async Task<IActionResult> GetMoviesCount()
        {
            var count = await _context.Movies.CountAsync();
            return Ok($"{count}");
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

        [HttpPost("bulk")]
        public async Task<IActionResult> AddMoviesBulk([FromBody] List<MovieRequestDto> movieDtos)
        {
            var movies = new List<Movie>();
            using var client = new HttpClient();

            foreach (var dto in movieDtos)
            {
                var movie = new Movie
                {
                    Title = dto.Title,
                    Genre = dto.Genre,
                    Grade = dto.Grade,
                    Rating = dto.Rating,
                    Hour = dto.Hour,
                    Min = dto.Min,
                    BoxOffice = dto.BoxOffice,
                    Budget = dto.Budget,
                    ReleaseDate = dto.ReleaseDate,
                    Recomended = dto.Recomended,
                    Running = dto.Running,
                    PosterUrl = dto.PosterUrl,
                    WidePosterUrl = dto.WidePosterUrl
                };

                if (!string.IsNullOrEmpty(dto.PosterUrl))
                {
                    try
                    {
                        var data = await client.GetByteArrayAsync(dto.PosterUrl);
                        movie.PosterData = data;
                    }
                    catch { /* Log error or skip binary, but URL is already set */ }
                }

                if (!string.IsNullOrEmpty(dto.WidePosterUrl))
                {
                    try
                    {
                        var data = await client.GetByteArrayAsync(dto.WidePosterUrl);
                        movie.WidePosterData = data;
                    }
                    catch { /* Log error or skip binary, but URL is already set */ }
                }

                movies.Add(movie);
            }

            _context.Movies.AddRange(movies);
            await _context.SaveChangesAsync();

            return Ok($"{movies.Count} movies added successfully.");
        }

        public class MovieRequestDto
        {
            public string? Title { get; set; }
            public List<string> Genre { get; set; } = new();
            public string? Grade { get; set; }
            public double Rating { get; set; }
            public int Hour { get; set; }
            public int Min { get; set; }
            public decimal? BoxOffice { get; set; }
            public decimal? Budget { get; set; }
            public DateTime ReleaseDate { get; set; }
            public string? PosterUrl { get; set; }
            public string? WidePosterUrl { get; set; }
            public bool Recomended { get; set; }
            public bool Running { get; set; } = true;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> AddMovieWithImages([FromForm] Movie movie, IFormFile? poster, IFormFile? widePoster)
        {
            if (poster != null)
            {
                using var ms = new MemoryStream();
                await poster.CopyToAsync(ms);
                movie.PosterData = ms.ToArray();
            }

            if (widePoster != null)
            {
                using var ms = new MemoryStream();
                await widePoster.CopyToAsync(ms);
                movie.WidePosterData = ms.ToArray();
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] Movie movie)
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
            movie.Recomended = updatedMovie.Recomended;
            movie.ReleaseDate = updatedMovie.ReleaseDate;
            movie.Running = updatedMovie.Running;
            movie.Min = updatedMovie.Min;
            movie.PosterData = updatedMovie.PosterData;
            movie.WidePosterData = updatedMovie.WidePosterData;

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



        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMovieWithImages(int id, [FromForm] Movie updatedMovie, IFormFile? poster, IFormFile? widePoster)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            movie.Title = updatedMovie.Title;
            movie.Genre = updatedMovie.Genre;
            movie.Grade = updatedMovie.Grade;
            movie.Rating = updatedMovie.Rating;
            movie.Hour = updatedMovie.Hour;
            movie.Min = updatedMovie.Min;
            movie.BoxOffice = updatedMovie.BoxOffice;
            movie.Budget = updatedMovie.Budget;
            movie.ReleaseDate = updatedMovie.ReleaseDate;
            movie.Recomended = updatedMovie.Recomended;
            movie.Running = updatedMovie.Running;
            movie.PosterUrl = updatedMovie.PosterUrl;
            movie.WidePosterUrl = updatedMovie.WidePosterUrl;

            if (poster != null)
            {
                using var ms = new MemoryStream();
                await poster.CopyToAsync(ms);
                movie.PosterData = ms.ToArray();
            }

            if (widePoster != null)
            {
                using var ms = new MemoryStream();
                await widePoster.CopyToAsync(ms);
                movie.WidePosterData = ms.ToArray();
            }

            await _context.SaveChangesAsync();
            return Ok(movie);
        }
    }
}
