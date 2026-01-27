using MiChitra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MiChitra.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                _logger.LogInformation("User registration requested");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning("Registration failed: Invalid model state");
                    return BadRequest(new { message = "Invalid data", errors });
                }

                var response = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("User registered successfully: {Username}", response.Username);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registration failed: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for username: {Username}", registerDto.Username);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("Login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation("Login requested for username: {Username}", loginDto.Username);
                var response = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("Login successful for username: {Username}", response.Username);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed: {Message}", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for username: {Username}", loginDto.Username);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
