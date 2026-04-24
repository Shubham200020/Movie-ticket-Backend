using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_movie_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowController : ControllerBase
    {


        private readonly AppDbContext _context;

        public ShowController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE Show
        [HttpPost]
        public async Task<IActionResult> Create(Show show)
        {
            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return Ok(show); // ✅ works now
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shows = await _context.Shows
                .Include(s => s.Movie)
             
                .ToListAsync();

            return Ok(shows);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var show = await _context.Shows
                .Include(s => s.Movie)
                
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
                return NotFound();

            return Ok(show);
        }

     
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Show updatedShow)
        {
            var show = await _context.Shows.FindAsync(id);

            if (show == null)
                return NotFound();

            show.ShowTime = updatedShow.ShowTime;
            show.MovieId = updatedShow.MovieId;
            show.CinemaId = updatedShow.CinemaId;

            await _context.SaveChangesAsync();

            return Ok(show);
        }

        // ✅ DELETE Show
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var show = await _context.Shows.FindAsync(id);

            if (show == null)
                return NotFound();

            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return Ok("Show deleted successfully");
        }
    }  
}
