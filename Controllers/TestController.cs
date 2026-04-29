using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using dotnet_movie_api.Databace;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("check-db")]
    public async Task<IActionResult> CheckDb()
    {
        try
        {
            bool canConnect = await _context.Database.CanConnectAsync();

            if (canConnect)
                return Ok("✅ Database Connected Successfully");

            return BadRequest("❌ Database Not Connected");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}