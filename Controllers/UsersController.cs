using MiChitra.DTOs;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any authenticated user; fine-grained checks inside
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ADMIN ONLY: View all users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // USER: only own profile
        // ADMIN: any profile
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var loggedInUserId))
                return Unauthorized("Invalid user identifier in token.");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && id != loggedInUserId)
                return Forbid("You can only access your own profile.");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // USER: update only own profile
        // ADMIN: update any user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var loggedInUserId))
                return Unauthorized("Invalid user identifier in token.");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && id != loggedInUserId)
                return Forbid("You can only update your own profile.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _userService.UpdateUserAsync(id, dto);
                if (!success)
                    return NotFound("User not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Logged-in user profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
                return Unauthorized("Invalid user identifier in token.");

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // ADMIN ONLY: Deactivate any user
        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            if (!success)
                return NotFound("User not found");

            return NoContent();
        }
    }
}
