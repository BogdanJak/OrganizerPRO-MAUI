using OrganizerPRO.Application.Common.Interfaces;

namespace OrganizerPRO.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IApplicationDbContext _context;

    public AuthController(IConfiguration config, IApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginRequest request)
    //{
    //    var user = await _context.Users
    //                             .Include(u => u.Roles)
    //                             .SingleOrDefaultAsync(u => u.Username == request.Username);

    //    // Wersja uproszczona, haszowanie hasła jest w aplikacji produkcyjnej obowiązkowe.
    //    if (user == null || user.PasswordHash != request.Password)
    //    {
    //        return Unauthorized();
    //    }

    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
    //    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };

    //    foreach (var role in user.Roles)
    //    {
    //        claims.Add(new Claim(ClaimTypes.Role, role.Name));
    //    }

    //    var tokenDescriptor = new SecurityTokenDescriptor
    //    {
    //        Subject = new ClaimsIdentity(claims),
    //        Expires = DateTime.UtcNow.AddHours(1),
    //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //    };
    //    var token = tokenHandler.CreateToken(tokenDescriptor);
    //    return Ok(new { Token = tokenHandler.WriteToken(token) });
    //}
}
