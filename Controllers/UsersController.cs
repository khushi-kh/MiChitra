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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // View all users (any authenticated user; frontend restricts to admins)
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
        [HttpPut("{id:int}")]
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

        // Deactivate any user (any authenticated user; frontend restricts to admins)
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            if (!success)
                return NotFound("User not found");

            return NoContent();
        }

        // Change user role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest("Role is required.");

            if (!Enum.TryParse<MiChitra.Models.UserRole>(dto.Role, out var role))
                return BadRequest("Invalid role value. Allowed: Admin, User, Guest.");

            try
            {
                var success = await _userService.UpdateUserRoleAsync(id, role);
                if (!success)
                    return NotFound("User not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user {UserId}", id);
                return StatusCode(500, "Failed to update user role.");
            }
        }
        // Forgot password without being logged in (by username)
        [HttpPut("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetByUsernameAsync(dto.Username);
            if (user == null)
                return NotFound("User not found.");

            try
            {
                var success = await _userService.ForgotPasswordAsync(user.UserId, dto.CurrentPassword, dto.NewPassword);
                if (!success)
                    return NotFound("User not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Reset password for logged-in user
        [HttpPut("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
                return Unauthorized("Invalid user identifier in token.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _userService.ForgotPasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                if (!success)
                    return NotFound("User not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
