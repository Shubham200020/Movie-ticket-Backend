using dotnet_movie_api.Databace;
using dotnet_movie_api.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AdminControllers : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    // ✅ Constructor
    public AdminControllers(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 🔐 👉 PUT YOUR METHOD HERE
    private string GenerateJwtToken(Admin admin)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Email),
            new Claim("id", admin.Id.ToString()),
            new Claim("name", admin.Name ?? "")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Admin admin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 🔐 Hash password
        admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(admin);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var admins = await _context.Admins.ToListAsync();
        return Ok(admins);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
            return NotFound();

        return Ok(admin);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Admin updatedAdmin)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
            return NotFound();

        admin.Name = updatedAdmin.Name;
        admin.Email = updatedAdmin.Email;

        // 🔐 Always hash password
        admin.Password = BCrypt.Net.BCrypt.HashPassword(updatedAdmin.Password);

        await _context.SaveChangesAsync();

        return Ok(admin);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
            return NotFound();

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return Ok("Admin deleted successfully");
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Admin loginData)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == loginData.Email); // 👈 HERE

        if (admin == null)
            return Unauthorized("Invalid email or password");

        bool isValid = BCrypt.Net.BCrypt.Verify(loginData.Password, admin.Password);

        if (!isValid)
            return Unauthorized("Invalid email or password");

        var token = GenerateJwtToken(admin);

        return Ok(new
        {
            token,
            admin.Id,
            admin.Name,
            admin.Email,
            admin.Role
        });
    }
}