using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ IMPORTANT

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Booking (Get all bookings)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.SelectedSeats).ThenInclude(bs => bs.Seat)
                .Include(b => b.User)
                .ToListAsync();
        }

        // ✅ GET: api/Booking/5 (Get Bill/Booking Details)
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theater)
                .Include(b => b.SelectedSeats).ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return booking;
        }

        // ✅ POST: api/Booking (Create Booking)
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(BookingRequestDto dto)
        {
            // 1. Validate User
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest("Invalid UserId.");

            // 2. Validate Showtime
            var showtime = await _context.Showtimes
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(s => s.Id == dto.ShowtimeId);
            if (showtime == null) return BadRequest("Invalid ShowtimeId.");

            // 3. Validate Seats exist and belong to the Screen
            var seats = await _context.Seats
                .Where(s => dto.SeatIds.Contains(s.Id) && s.ScreenId == showtime.ScreenId)
                .ToListAsync();

            if (seats.Count != dto.SeatIds.Count)
                return BadRequest("Some seats are invalid or don't belong to this screen.");

            // 4. Check if seats are already booked for THIS showtime
            var alreadyBooked = await _context.BookingSeats
                .Include(bs => bs.Booking)
                .AnyAsync(bs => bs.Booking.ShowtimeId == dto.ShowtimeId && dto.SeatIds.Contains(bs.SeatId));

            if (alreadyBooked)
                return BadRequest("One or more seats are already booked for this show.");

            // 5. Create Booking
            var booking = new Booking
            {
                UserId = dto.UserId,
                ShowtimeId = dto.ShowtimeId,
                BookingTime = DateTime.Now,
                TotalAmount = showtime.BasePrice * dto.SeatIds.Count,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // 6. Create BookingSeats
            var bookingSeats = dto.SeatIds.Select(seatId => new BookingSeat
            {
                BookingId = booking.Id,
                SeatId = seatId
            }).ToList();

            _context.BookingSeats.AddRange(bookingSeats);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }
    }
}