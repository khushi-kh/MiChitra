using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserService userService, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginDTO dto)
        {
            var user = await _userService.GetByUsernameAsync(dto.Username);
            
            // If not found by username, try email
            if (user == null)
            {
                user = await _userService.GetByEmailAsync(dto.Username);
            }
            
            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", dto.Username);
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            
            _logger.LogInformation("Attempting password verification for user: {Username}", user.Username);
            _logger.LogInformation("Stored hash starts with: {HashPrefix}", user.PasswordHash?.Substring(0, Math.Min(10, user.PasswordHash?.Length ?? 0)));
            
            bool passwordValid;
            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BCrypt verification failed for user: {Username}", user.Username);
                throw new UnauthorizedAccessException("Invalid username or password");
            }
            
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid login attempt for username: {Username}", dto.Username);
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Username}", dto.Username);
                throw new UnauthorizedAccessException("Account is deactivated");
            }

            var token = _jwtService.GenerateToken(user);
            
            _logger.LogInformation("User logged in successfully: {UserId}", user.UserId);
            
            return new AuthResponse
            {
                Token = token,
                User = new UserResponseDTO
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
                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                var userResponse = await _userService.CreateUserAsync(dto);
                var user = await _userService.GetByUsernameAsync(dto.Username);
                
                if (user == null)
                    throw new InvalidOperationException("User creation failed");

                var token = _jwtService.GenerateToken(user);
                
                _logger.LogInformation("User registered successfully: {UserId}", user.UserId);
                
                return new AuthResponse
                {
                    Token = token,
                    User = userResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for username: {Username}", dto.Username);
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _jwtService.ValidateToken(token);
                return principal != null;
            }
            catch
            {
                return false;
            }
        }
    }
}