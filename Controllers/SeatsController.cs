using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;

namespace dotnet_movie_api.Controllers
{
    public class BulkSeatCreateDto
    {
        public int ScreenId { get; set; }
        public string Row { get; set; } = string.Empty;
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }
        public string SeatType { get; set; } = "Regular";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeatsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/seats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats()
        {
            return await _context.Seats.Include(s => s.Screen).ToListAsync();
        }

        // ✅ GET: api/seats/count
        [HttpGet("count")]
        public async Task<int> GetCount()
        {
            return await _context.Seats.CountAsync();
        }

        // ✅ GET: api/seats/screen/5
        [HttpGet("screen/{screenId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeatsByScreen(int screenId)
        {
            return await _context.Seats
                .Where(s => s.ScreenId == screenId)
                .ToListAsync();
        }

        // ✅ POST: api/seats
        [HttpPost]
        public async Task<ActionResult<Seat>> CreateSeat(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSeats), new { id = seat.Id }, seat);
        }

        // ✅ POST: api/seats/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateSeats(BulkSeatCreateDto dto)
        {
            if (dto.StartNumber > dto.EndNumber)
                return BadRequest("Start number must be less than or equal to end number.");

            var screenExists = await _context.Screens.AnyAsync(s => s.Id == dto.ScreenId);
            if (!screenExists) return BadRequest("Invalid ScreenId.");

            var seats = new List<Seat>();
            for (int i = dto.StartNumber; i <= dto.EndNumber; i++)
            {
                seats.Add(new Seat
                {
                    ScreenId = dto.ScreenId,
                    Row = dto.Row,
                    Number = i,
                    SeatType = dto.SeatType
                });
            }

            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return Ok($"{seats.Count} seats created successfully.");
        }

        // ✅ DELETE: api/seats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: api/seats/screen/5
        [HttpDelete("screen/{screenId}")]
        public async Task<IActionResult> DeleteSeatsByScreen(int screenId)
        {
            var seats = await _context.Seats.Where(s => s.ScreenId == screenId).ToListAsync();
            _context.Seats.RemoveRange(seats);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
