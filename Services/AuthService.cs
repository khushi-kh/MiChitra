using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDTO dto)
        {
            // Check if username already exists
            if (await _unitOfWork.Users.ExistsAsync(u => u.Username == dto.Username))
                throw new InvalidOperationException("Username already exists");

            // Check if email already exists
            if (await _unitOfWork.Users.ExistsAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginDTO dto)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password");

            // Verify hashed password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password");

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}