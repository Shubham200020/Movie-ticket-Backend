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

        // Helper to log model state errors
        private IActionResult ValidationFailed()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Validation Errors: " + string.Join(" | ", errors));
            return BadRequest(ModelState);
        }

        // ✅ POST: api/showtime
        [HttpPost]
        public async Task<IActionResult> CreateShowtime(ShowtimeRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationFailed();

            if (!DateTime.TryParse(dto.StartTime, out var start) || !DateTime.TryParse(dto.EndTime, out var end))
                return BadRequest("Invalid date format. Use YYYY-MM-DDTHH:mm");

            // 1. Basic Validation
            if (start >= end)
                return BadRequest("Start time must be before End time.");

            // 2. Check if Movie and Screen exist
            var movieExists = await _context.Movies.AnyAsync(m => m.Id == dto.MovieId);
            var screenExists = await _context.Screens.AnyAsync(s => s.Id == dto.ScreenId);

            if (!movieExists || !screenExists)
                return BadRequest("Invalid MovieId or ScreenId.");

            // 3. Overlap Check
            var isOverlapping = await _context.Showtimes.AnyAsync(s =>
                s.ScreenId == dto.ScreenId &&
                ((start >= s.StartTime && start < s.EndTime) ||
                 (end > s.StartTime && end <= s.EndTime) ||
                 (start <= s.StartTime && end >= s.EndTime)));

            if (isOverlapping)
                return BadRequest("This showtime overlaps with another show in the same screen.");

            var showtime = new Showtime
            {
                StartTime = start,
                EndTime = end,
                MovieId = dto.MovieId,
                ScreenId = dto.ScreenId
            };

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

        public class ShowtimeRequestDto
        {
            public int Id { get; set; }
            public string StartTime { get; set; } = string.Empty;
            public string EndTime { get; set; } = string.Empty;
            public int MovieId { get; set; }
            public int ScreenId { get; set; }
        }

        // ✅ PUT: api/showtime/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShowtime(int id, ShowtimeRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationFailed();
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            if (!DateTime.TryParse(dto.StartTime, out var start) || !DateTime.TryParse(dto.EndTime, out var end))
                return BadRequest("Invalid date format. Use YYYY-MM-DDTHH:mm");

            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            if (start >= end)
                return BadRequest("Start time must be before End time.");

            // Overlap Check (excluding current showtime)
            var isOverlapping = await _context.Showtimes.AnyAsync(s =>
                s.Id != id &&
                s.ScreenId == dto.ScreenId &&
                ((start >= s.StartTime && start < s.EndTime) ||
                 (end > s.StartTime && end <= s.EndTime) ||
                 (start <= s.StartTime && end >= s.EndTime)));

            if (isOverlapping)
                return BadRequest("This showtime overlaps with another show in the same screen.");

            // Update fields manually
            showtime.StartTime = start;
            showtime.EndTime = end;
            showtime.MovieId = dto.MovieId;
            showtime.ScreenId = dto.ScreenId;

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
                IsAvailable = !bookedSeatIds.Contains(s.Id),
                Price = s.Price
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

