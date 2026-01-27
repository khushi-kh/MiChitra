using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterDTO dto);
        Task<AuthResponse> LoginAsync(LoginDTO dto);
    }
}