using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingSeatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingSeatController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingSeat>>> GetAll()
        {
            return await _context.BookingSeats
                .Include(bs => bs.Booking)
                .Include(bs => bs.Seat)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingSeat>> GetById(int id)
        {
            var bookingSeat = await _context.BookingSeats
                .Include(bs => bs.Booking)
                .Include(bs => bs.Seat)
                .FirstOrDefaultAsync(bs => bs.Id == id);

            if (bookingSeat == null)
                return NotFound();

            return bookingSeat;
        }

       
        [HttpPost]
        public async Task<ActionResult<BookingSeat>> Create(BookingSeat bookingSeat)
        {
            // 🔒 Prevent double booking
            var alreadyBooked = await _context.BookingSeats
                .AnyAsync(bs => bs.SeatId == bookingSeat.SeatId);

            if (alreadyBooked)
                return BadRequest("Seat already booked!");

            _context.BookingSeats.Add(bookingSeat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bookingSeat.Id }, bookingSeat);
        }

        
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateMultiple(List<BookingSeat> bookingSeats)
        {
            var seatIds = bookingSeats.Select(bs => bs.SeatId).ToList();

            // 🔒 Check already booked seats
            var bookedSeats = await _context.BookingSeats
                .Where(bs => seatIds.Contains(bs.SeatId))
                .Select(bs => bs.SeatId)
                .ToListAsync();

            if (bookedSeats.Any())
            {
                return BadRequest($"Seats already booked: {string.Join(",", bookedSeats)}");
            }

            await _context.BookingSeats.AddRangeAsync(bookingSeats);
            await _context.SaveChangesAsync();

            return Ok("Seats booked successfully");
        }

        // ✅ DELETE (Cancel Seat)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bookingSeat = await _context.BookingSeats.FindAsync(id);

            if (bookingSeat == null)
                return NotFound();

            _context.BookingSeats.Remove(bookingSeat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
