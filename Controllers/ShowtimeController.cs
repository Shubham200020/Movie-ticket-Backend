using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ShowtimeController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/showtime
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimes()
        {
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .ToListAsync();
        }

        // ✅ GET: api/showtime/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Showtime>> GetShowtime(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
                return NotFound();

            return showtime;
        }

        // ✅ POST: api/showtime
        [HttpPost]
        public async Task<ActionResult<Showtime>> CreateShowtime(Showtime showtime)
        {
            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtime);
        }

        // ✅ PUT: api/showtime/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowtime(int id, Showtime showtime)
        {
            if (id != showtime.Id)
                return BadRequest();

            _context.Entry(showtime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Showtimes.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/showtime/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowtime(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);

            if (showtime == null)
                return NotFound();

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

