using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScreenController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/screen
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var screens = await _context.Screens
                .Include(s => s.Theater)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Capacity,
                    s.TheaterId,
                    TheaterName = s.Theater.Name
                })
                .ToListAsync();

            return Ok(screens);
        }

        // ✅ GET: api/screen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Screen>> GetScreen(int id)
        {
            var screen = await _context.Screens
                .Include(s => s.Theater)
                .Include(s => s.Seats)
                .Include(s => s.Showtimes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screen == null)
                return NotFound();

            return screen;
        }

        // ✅ POST: api/screen
        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> Create(ScreenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Optional: check if Theater exists
            var theaterExists = await _context.Theaters
                .AnyAsync(t => t.Id == dto.TheaterId);

            if (!theaterExists)
                return BadRequest("Invalid TheaterId ❌");

            var screen = new Screen
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                TheaterId = dto.TheaterId
            };

            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();

            // ✅ Fetch with Theater (fix null issue)
            var result = await _context.Screens
                .Include(s => s.Theater)
                .Where(s => s.Id == screen.Id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Capacity,
                    s.TheaterId,
                    TheaterName = s.Theater.Name
                })
                .FirstOrDefaultAsync();

            return Ok(result);
        }

        // ✅ PUT: api/screen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScreen(int id, ScreenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var screen = await _context.Screens.FindAsync(id);

            if (screen == null)
                return NotFound();

            // ✅ Validate Theater
            var theaterExists = await _context.Theaters
                .AnyAsync(t => t.Id == dto.TheaterId);

            if (!theaterExists)
                return BadRequest("Invalid TheaterId ❌");

            // ✅ Update fields safely
            screen.Name = dto.Name;
            screen.Capacity = dto.Capacity;
            screen.TheaterId = dto.TheaterId;

            await _context.SaveChangesAsync();

            // ✅ Return clean response (no circular issue)
            var result = await _context.Screens
                .Include(s => s.Theater)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Capacity,
                    s.TheaterId,
                    TheaterName = s.Theater.Name
                })
                .FirstOrDefaultAsync();

            return Ok(result);
        }
        // ✅ DELETE: api/screen/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            var screen = await _context.Screens.FindAsync(id);

            if (screen == null)
                return NotFound();

            _context.Screens.Remove(screen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ GET: api/screen/by-theater/1
        [HttpGet("by-theater/{theaterId}")]
        public async Task<ActionResult<IEnumerable<Screen>>> GetByTheater(int theaterId)
        {
            return await _context.Screens
                .Where(s => s.TheaterId == theaterId)
                .Include(s => s.Theater)
                .ToListAsync();
        }

        // ✅ GET: api/screen/count
        [HttpGet("count")]
        public async Task<int> GetCount()
        {
            return await _context.Screens.CountAsync();
        }
    }
}
