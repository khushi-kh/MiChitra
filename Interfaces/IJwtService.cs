using System.Security.Claims;
using MiChitra.Models;

namespace MiChitra.Interfaces
{
    public interface IJwtService
    {
        // Generates a JWT token for the given user
        string GenerateToken(User user);
        // Validates the given JWT token and returns the claims principal if valid
        ClaimsPrincipal? ValidateToken(string token);
        // Extracts the user ID from the given JWT token
        int? GetUserIdFromToken(string token);
        // Extracts the user role from the given JWT token
        UserRole? GetUserRoleFromToken(string token);
    }
}
