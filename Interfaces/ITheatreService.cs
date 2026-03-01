using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface ITheatreService
    {
        Task<IEnumerable<TheatreResponseDTO>> GetAllTheatresAsync();
        Task<TheatreResponseDTO?> GetTheatreByIdAsync(int id);
        Task<IEnumerable<TheatreResponseDTO>> GetTheatresByCityAsync(string city);
        Task<TheatreResponseDTO> CreateTheatreAsync(CreateTheatreDTO dto);
        Task<bool> UpdateTheatreAsync(int id, UpdateTheatreDTO dto);
        Task<bool> DeactivateTheatreAsync(int id);
        Task<IEnumerable<string>> GetAllCitiesAsync();
    }
}