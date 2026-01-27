using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetActiveUsersAsync();
            return users.Select(u => new UserResponseDTO
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
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || !user.IsActive) return null;

            return new UserResponseDTO
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
        }

        public async Task<UserResponseDTO> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || !user.IsActive)
                throw new InvalidOperationException("User not found");

            user.FName = dto.FName;
            user.LName = dto.LName;
            user.Username = dto.Username;
            user.ContactNumber = dto.ContactNumber;
            user.Email = dto.Email;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDTO
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
        }
    }
}