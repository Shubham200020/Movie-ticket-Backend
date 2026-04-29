using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TheaterController(AppDbContext context)
        {
            _context = context;
        }

        // 1. READ ALL: GET api/theator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Theater>>> GetTheators()
        {

            var theaters = await _context.Theaters
       .Include(t => t.Location)   // ✅ ADD THIS
       .Include(t => t.Screens)
       .ToListAsync();

            return Ok(theaters);
        }
        [HttpGet("theator-count")]
        public async Task<IActionResult> GetMoviesCount()
        {
            var count = await _context.Theaters.CountAsync();
            return Ok($"{count}");
        }
        // 2. READ ONE: GET api/theator/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Theater>> GetTheator(int id)
        {
            var theator = await _context.Theaters
                .Include(t => t.Screens)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (theator == null) return NotFound();
            return theator;
        }
        [HttpGet("by-location/{locationId}")]
        public async Task<ActionResult<IEnumerable<Theater>>> GetByLocation(int locationId)
        {
            var theaters = await _context.Theaters
                .Where(t => t.LocationId == locationId)
                .Include(t => t.Location)
                .Include(t => t.Screens)
                .ToListAsync();

            if (!theaters.Any())
                return NotFound("No theaters found");

            return Ok(theaters); // ✅ correct
        }
        // 3. CREATE: POST api/theator
        [HttpPost]
        public async Task<ActionResult<Theater>> PostTheator(Theater theator)
        {
            // 🔥 VERY IMPORTANT FIX
            if (theator.LocationId != null)
            {
                var location = await _context.Locations.FindAsync(theator.LocationId);

                if (location == null)
                    return BadRequest("Invalid LocationId");

                theator.Location = location; // attach existing location
            }

            _context.Theaters.Add(theator);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheator), new { id = theator.Id }, theator);
        }

        // 4. UPDATE: PUT api/theator/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTheator(int id, Theater theator)
        {
            if (id != theator.Id) return BadRequest();

            if (theator.LocationId != null)
            {
                var location = await _context.Locations.FindAsync(theator.LocationId);
                if (location == null)
                    return BadRequest("Invalid LocationId");

                theator.Location = location;
            }

            _context.Entry(theator).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 5. DELETE: DELETE api/theator/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheator(int id)
        {
            var theator = await _context.Theaters.FindAsync(id);
            if (theator == null) return NotFound();

            _context.Theaters.Remove(theator);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TheatorExists(int id)
        {
            return _context.Theaters.Any(e => e.Id == id);
        }
    }
}
