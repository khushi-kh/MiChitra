using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync();
        Task<MovieResponseDto?> GetMovieByIdAsync(int id);
        Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto);
        Task<bool> UpdateMovieAsync(int id, UpdateMovieDto dto);
        Task<bool> DeleteMovieAsync(int id);
    }
}