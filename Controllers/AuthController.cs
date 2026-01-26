using MiChitra.Data;
using MiChitra.Interfaces;
using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using MiChitra.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;


namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            MiChitraDbContext context,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                _logger.LogInformation("User registration requested");

                // validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning("Registration failed: Invalid model state");
                    return BadRequest(new { message = "Invalid data", errors });
                }

                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                {
                    _logger.LogWarning("Registration failed: Username {Username} already exists", registerDto.Username);
                    return Conflict(new { message = "Username already exists" });
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    _logger.LogWarning("Registration failed: Email already exists");
                    return Conflict(new { message = "Email already exists" });
                }
                
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRole.User
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Username}", user.Username);

                var token = _jwtService.GenerateToken(user);

                return Ok(new AuthResponse
                {
                    Token = token,
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for username: {Username}", registerDto.Username);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation("Login requested for username: {Username}", loginDto.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("Login failed: Username not found");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Verify hashed password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Incorrect password");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Login successful for username: {Username}", user.Username);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            });
        }
    }
}
