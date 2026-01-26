using MiChitra.Interfaces;
using Microsoft.Identity.Client;
using MiChitra.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;  
using System.Security.Claims;
using System.Text;

namespace MiChitra.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load JWT settings from configuration
            _secretKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:SecretKey is not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured");
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is not configured");
            _expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 60);

            // Log the loaded configuration values (excluding sensitive information)
            _logger.LogInformation("JWT Service initialized with Issuer: {Issuer}, Audience: {Audience}, ExpiryMinutes: {ExpiryMinutes}", _issuer, _audience, _expiryMinutes);

            // Validate key length
            if (string.IsNullOrEmpty(_secretKey) || _secretKey.Length < 32)
            {
                throw new InvalidOperationException("Jwt:SecretKey must be at least 32 characters long for security reasons.");
            }

        }

        // Generates a JWT token for the given user
        public string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("role", user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                    signingCredentials: credentials
                );
                _logger.LogInformation("JWT token generated successfully for user {UserId}", user.UserId);

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.UserId);
                throw;
            }
        }

        // Validates the given JWT token and returns the claims principal if valid
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromMinutes(1) // Reduce default 5 minutes tolerance to 1 minute
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                // Ensure the token is a JWT token
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Invalid JWT token algorithm");
                    return null;
                }
                _logger.LogInformation("JWT token validated successfully");
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT token expired");
                return null;
            }
            catch (SecurityTokenException)
            {
                _logger.LogWarning("Invalid JWT token");
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                return null;
            }
        }
        // Extracts the user ID from the given JWT token
        public int? GetUserIdFromToken(string token)
        {
            try { 
                var principal = ValidateToken(token);
                if (principal == null)
                {
                    return null;
                }
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning("User ID claim not found or invalid in JWT token");
                    return null;
                }
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting User ID from JWT token");
                return null;
            }
        }
        // Extracts the user role from the given JWT token
        public UserRole? GetUserRoleFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                if (principal == null)
                {
                    return null;
                }
                var roleClaim = principal.FindFirst("role");
                if (roleClaim == null || !Enum.TryParse<UserRole>(roleClaim.Value, out var role))
                {
                    _logger.LogWarning("User role claim not found or invalid in JWT token");
                    return null;
                }
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting User Role from JWT token");
                return null;
            }
        }
    }
}