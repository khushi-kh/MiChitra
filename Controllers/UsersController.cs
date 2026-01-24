using MiChitra.Data;
using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiChitra.DTOs;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(MiChitraDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. GET: api/users
        // Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            return Ok(users);
        }

        // 2. GET: api/users/{id}
        // Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || !user.IsActive)
                return NotFound("User not found");

            return Ok(user);
        }

        // 3. PUT: api/users/{id}
        // Update user profile (no password update here)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || !user.IsActive)
                return NotFound("User not found");

            user.FName = dto.FName;
            user.LName = dto.LName;
            user.Username = dto.Username;
            user.ContactNumber = dto.ContactNumber;
            user.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }

}
