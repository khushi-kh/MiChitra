using MiChitra.DTOs;

using System.Collections.Generic;
using System.Threading.Tasks;
using MiChitra.Models;

namespace MiChitra.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO?> GetUserByIdAsync(int id);
        Task<UserResponseDTO> CreateUserAsync(RegisterDTO dto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO dto);
        Task<bool> DeactivateUserAsync(int id);

        // Added: helpers used by AuthService and others
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}