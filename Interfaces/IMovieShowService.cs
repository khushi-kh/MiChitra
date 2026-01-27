using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IMovieShowService
    {
        Task<IEnumerable<MovieShowResponseDTO>> GetAllShowsAsync();
        Task<MovieShowResponseDTO?> GetShowByIdAsync(int id);
        Task<IEnumerable<MovieShowResponseDTO>> GetShowsByMovieAsync(int movieId);
        Task<IEnumerable<MovieShowResponseDTO>> GetShowsByTheatreAsync(int theatreId);
        Task<IEnumerable<MovieShowResponseDTO>> GetShowsByCityAsync(string city);
        Task<MovieShowResponseDTO> CreateShowAsync(CreateMovieShowDto dto);
        Task<bool> UpdateShowAsync(int id, UpdateMovieShowDto dto);
        Task<bool> DeleteShowAsync(int id);
    }
}