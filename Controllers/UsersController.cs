using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using MiChitra.DTOs;
using MiChitra.Interfaces;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _unitOfWork.Users.GetActiveUsersAsync();
            var userDtos = users.Select(u => new UserResponseDTO
            {
                UserId = u.UserId,
                FName = u.FName,
                LName = u.LName,
                Username = u.Username,
                Email = u.Email,
                ContactNumber = u.ContactNumber,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                IsActive = u.IsActive
            });
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || !user.IsActive)
                return NotFound("User not found");
            
            var userDto = new UserResponseDTO
            {
                UserId = user.UserId,
                FName = user.FName,
                LName = user.LName,
                Username = user.Username,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || !user.IsActive)
                return NotFound("User not found");

            user.FName = dto.FName;
            user.LName = dto.LName;
            user.Username = dto.Username;
            user.ContactNumber = dto.ContactNumber;
            user.Email = dto.Email;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserResponseDTO
            {
                UserId = user.UserId,
                FName = user.FName,
                LName = user.LName,
                Username = user.Username,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
            return Ok(userDto);
        }
    }

}
