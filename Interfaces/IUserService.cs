using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO?> GetUserByIdAsync(int id);
        Task<UserResponseDTO> UpdateUserAsync(int id, UpdateUserDto dto);
    }
}