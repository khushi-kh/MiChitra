using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class UserService : IUserService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(MiChitraDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToResponseDto);
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : MapToResponseDto(user);
        }

        public async Task<UserResponseDTO> CreateUserAsync(RegisterDTO dto)
        {
            // Check if username or email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            
            if (existingUser != null)
                throw new InvalidOperationException("Username or email already exists");

            var user = new User
            {
                FName = dto.FName,
                LName = dto.LName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                ContactNumber = dto.ContactNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User created with ID: {UserId}", user.UserId);
            return MapToResponseDto(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // Check if new email is already taken by another user
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email && u.UserId != id);
                if (emailExists)
                    throw new InvalidOperationException("Email already exists");
            }

            user.FName = dto.FName ?? user.FName;
            user.LName = dto.LName ?? user.LName;
            user.Email = dto.Email ?? user.Email;
            user.ContactNumber = dto.ContactNumber ?? user.ContactNumber;

            await _context.SaveChangesAsync();
            _logger.LogInformation("User updated with ID: {UserId}", id);
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation("User deactivated with ID: {UserId}", id);
            return true;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        private static UserResponseDTO MapToResponseDto(User user)
        {
            return new UserResponseDTO
            {
                UserId = user.UserId,
                FName = user.FName,
                LName = user.LName,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ContactNumber = user.ContactNumber,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }
    }
}