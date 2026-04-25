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
        public async Task<ActionResult<IEnumerable<Screen>>> GetScreens()
        {
            return await _context.Screens
                .Include(s => s.Theater)
                .ToListAsync();
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
        public async Task<ActionResult<Screen>> CreateScreen(Screen screen)
        {
            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScreen), new { id = screen.Id }, screen);
        }

        // ✅ PUT: api/screen/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScreen(int id, Screen screen)
        {
            if (id != screen.Id)
                return BadRequest();

            _context.Entry(screen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Screens.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
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
