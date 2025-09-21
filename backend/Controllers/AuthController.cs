using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // DTOs
    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // --- LOGIN ---
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == dto.Email && u.Password == dto.Password);
        if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });

        var token = GenerateToken(user);
        return Ok(new { token });
    }

    // --- REGISTER ---
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        // Validar duplicado
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest(new { message = "El email ya está registrado" });

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = "User"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var token = GenerateToken(user);
        return Ok(new { message = "Usuario creado con éxito", token });
    }

    // --- Generador de token ---
    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "EstaEsUnaClaveSuperSecreta12345"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "backend",
            audience: _config["Jwt:Audience"] ?? "backendUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
