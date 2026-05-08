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
                .Include(b => b.Admin)
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theater)
                .Include(b => b.SelectedSeats).ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return booking;
        }

        // ✅ POST: api/Booking (Create Booking with ACID Properties)
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(BookingRequestDto dto)
        {
            // 1. Validate Input
            if (dto.UserId == null && dto.AdminId == null) 
                return BadRequest("Either UserId or AdminId must be provided.");

            // Start a Transaction (Atomicity & Isolation)
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            
            try
            {
                // 2. Validate User/Admin Existence (Consistency)
                if (dto.UserId != null && !await _context.Users.AnyAsync(u => u.Id == dto.UserId))
                    return BadRequest("Invalid UserId.");
                
                if (dto.AdminId != null && !await _context.Admins.AnyAsync(a => a.Id == dto.AdminId))
                    return BadRequest("Invalid AdminId.");

                // 3. Validate Showtime (Consistency)
                var showtime = await _context.Showtimes
                    .Include(s => s.Screen)
                    .FirstOrDefaultAsync(s => s.Id == dto.ShowtimeId);
                if (showtime == null) return BadRequest("Invalid ShowtimeId.");

                // 4. Check if seats are already booked for THIS showtime (Isolation)
                // In Serializable isolation, this check is protected against concurrent modifications
                var alreadyBooked = await _context.BookingSeats
                    .Include(bs => bs.Booking)
                    .AnyAsync(bs => bs.Booking.ShowtimeId == dto.ShowtimeId && dto.SeatIds.Contains(bs.SeatId));

                if (alreadyBooked)
                    return BadRequest("One or more seats are already booked for this show.");

                // 5. Validate Seats belong to the Screen (Consistency)
                var seats = await _context.Seats
                    .Where(s => dto.SeatIds.Contains(s.Id) && s.ScreenId == showtime.ScreenId)
                    .ToListAsync();

                if (seats.Count != dto.SeatIds.Count)
                    return BadRequest("Some seats are invalid or don't belong to this screen.");

                // 6. Create Booking (Atomicity - Part 1)
                var booking = new Booking
                {
                    UserId = dto.UserId,
                    AdminId = dto.AdminId,
                    ShowtimeId = dto.ShowtimeId,
                    BookingTime = DateTime.Now,
                    TotalAmount = seats.Sum(s => s.Price),
                    Status = "Confirmed"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync(); // Save to get Booking.Id

                // 7. Create BookingSeats (Atomicity - Part 2)
                var bookingSeats = dto.SeatIds.Select(seatId => new BookingSeat
                {
                    BookingId = booking.Id,
                    SeatId = seatId
                }).ToList();

                _context.BookingSeats.AddRange(bookingSeats);
                await _context.SaveChangesAsync();

                // Commit Transaction (Durability)
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                // Rollback if anything fails (Atomicity)
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ✅ GET: api/Booking/user/5 (Get user's bookings)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theater)
                .Include(b => b.SelectedSeats).ThenInclude(bs => bs.Seat)
                .ToListAsync();
        }

        // ✅ GET: api/Booking/admin/{adminId} (Get admin's personal bookings)
        [HttpGet("admin/{adminId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAdminBookings(int adminId)
        {
            return await _context.Bookings
                .Where(b => b.AdminId == adminId)
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theater)
                .Include(b => b.SelectedSeats).ThenInclude(bs => bs.Seat)
                .ToListAsync();
        }
    }
}