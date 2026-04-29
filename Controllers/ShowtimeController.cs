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

        // ✅ GET: api/showtime/count
        [HttpGet("count")]
        public async Task<int> GetCount()
        {
            return await _context.Showtimes.CountAsync();
        }

        // ✅ GET: api/showtime/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Showtime>> GetShowtime(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Screen)
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.SelectedSeats)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null)
                return NotFound();

            return showtime;
        }

        // ✅ POST: api/showtime
        [HttpPost]
        public async Task<ActionResult<Showtime>> CreateShowtime(Showtime showtime)
        {
            // 1. Basic Validation
            if (showtime.StartTime >= showtime.EndTime)
                return BadRequest("Start time must be before End time.");

            // 2. Check if Movie and Screen exist
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == showtime.MovieId);
            var screenExists = await _context.Screens.AnyAsync(s => s.Id == showtime.ScreenId);

            if (!movieExists || !screenExists)
                return BadRequest("Invalid MovieId or ScreenId.");

            // 3. Overlap Check
            var isOverlapping = await _context.Showtimes.AnyAsync(s =>
                s.ScreenId == showtime.ScreenId &&
                ((showtime.StartTime >= s.StartTime && showtime.StartTime < s.EndTime) ||
                 (showtime.EndTime > s.StartTime && showtime.EndTime <= s.EndTime) ||
                 (showtime.StartTime <= s.StartTime && showtime.EndTime >= s.EndTime)));

            if (isOverlapping)
                return BadRequest("This showtime overlaps with another show in the same screen.");

            showtime.Movie = null;
            showtime.Screen = null;
            showtime.Bookings = null;

            try
            {
                _context.Showtimes.Add(showtime);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetShowtime), new { id = showtime.Id }, showtime);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating showtime: {ex.Message} {ex.InnerException?.Message}");
            }
        }

        // ✅ PUT: api/showtime/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowtime(int id, Showtime updatedShowtime)
        {
            if (id != updatedShowtime.Id)
                return BadRequest("ID mismatch.");

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            if (updatedShowtime.StartTime >= updatedShowtime.EndTime)
                return BadRequest("Start time must be before End time.");

            // Overlap Check (excluding current showtime)
            var isOverlapping = await _context.Showtimes.AnyAsync(s =>
                s.Id != id &&
                s.ScreenId == updatedShowtime.ScreenId &&
                ((updatedShowtime.StartTime >= s.StartTime && updatedShowtime.StartTime < s.EndTime) ||
                 (updatedShowtime.EndTime > s.StartTime && updatedShowtime.EndTime <= s.EndTime) ||
                 (updatedShowtime.StartTime <= s.StartTime && updatedShowtime.EndTime >= s.EndTime)));

            if (isOverlapping)
                return BadRequest("This showtime overlaps with another show in the same screen.");

            // Update fields manually to avoid issues with navigation properties
            showtime.StartTime = updatedShowtime.StartTime;
            showtime.EndTime = updatedShowtime.EndTime;
            showtime.BasePrice = updatedShowtime.BasePrice;
            showtime.MovieId = updatedShowtime.MovieId;
            showtime.ScreenId = updatedShowtime.ScreenId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(showtime);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating showtime: {ex.Message} {ex.InnerException?.Message}");
            }
        }

        // ✅ GET: api/showtime/5/seat-status
        [HttpGet("{id}/seat-status")]
        public async Task<ActionResult<IEnumerable<SeatResponseDto>>> GetSeatStatus(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Screen)
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.SelectedSeats)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showtime == null) return NotFound();

            // Get all seats for this screen
            var allSeats = await _context.Seats
                .Where(s => s.ScreenId == showtime.ScreenId)
                .ToListAsync();

            // Get IDs of all booked seats for this showtime
            var bookedSeatIds = showtime.Bookings
                .SelectMany(b => b.SelectedSeats)
                .Select(bs => bs.SeatId)
                .ToHashSet();

            var seatStatuses = allSeats.Select(s => new SeatResponseDto
            {
                Id = s.Id,
                Row = s.Row,
                Number = s.Number,
                IsAvailable = !bookedSeatIds.Contains(s.Id)
            }).ToList();

            return seatStatuses;
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

