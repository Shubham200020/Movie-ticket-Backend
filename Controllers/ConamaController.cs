using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.CinemaController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CinemaController(AppDbContext context)
        {
            _context = context;
        }

      
      /*  [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();

            return Ok(cinema);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.Movies) // optional (recommended)
                .ToListAsync();

            return Ok(cinemas);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Movies) // optional
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null)
                return NotFound();

            return Ok(cinema);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cinema updatedCinema)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
                return NotFound();

            cinema.Name = updatedCinema.Name;
            cinema.Location = updatedCinema.Location;
            cinema.TotalScreens = updatedCinema.TotalScreens;
           
            cinema.Description = updatedCinema.Description;

            await _context.SaveChangesAsync();

            return Ok(cinema);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
                return NotFound();

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            return Ok("Cinema deleted successfully");
        }*/
    }
}