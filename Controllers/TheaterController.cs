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
        public async Task<ActionResult<IEnumerable<Theator>>> GetTheators()
        {
            // We use the DbSet name 'Theaters' to fetch 'Theator' objects
            return await _context.Theaters
                .Include(t => t.Screens) // Eager loading associated screens
                .ToListAsync();
        }

        // 2. READ ONE: GET api/theator/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Theator>> GetTheator(int id)
        {
            var theator = await _context.Theaters
                .Include(t => t.Screens)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (theator == null) return NotFound();
            return theator;
        }

        // 3. CREATE: POST api/theator
        [HttpPost]
        public async Task<ActionResult<Theator>> PostTheator(Theator theator)
        {
            _context.Theaters.Add(theator);
            await _context.SaveChangesAsync();

            // Returns the 201 Created status and the location of the new resource
            return CreatedAtAction(nameof(GetTheator), new { id = theator.Id }, theator);
        }

        // 4. UPDATE: PUT api/theator/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTheator(int id, Theator theator)
        {
            if (id != theator.Id) return BadRequest("ID mismatch");

            _context.Entry(theator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheatorExists(id)) return NotFound();
                throw;
            }

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
