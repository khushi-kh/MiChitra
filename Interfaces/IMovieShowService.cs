using System.Collections.Generic;
using System.Threading.Tasks;
using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface IMovieShowService
    {
        Task<IEnumerable<MovieShowResponseDTO>> GetAllMovieShowsAsync();
        Task<MovieShowResponseDTO?> GetMovieShowByIdAsync(int id);
        Task<IEnumerable<MovieShowResponseDTO>> GetShowsByMovieIdAsync(int movieId);
        Task<IEnumerable<MovieShowResponseDTO>> GetShowsByTheatreIdAsync(int theatreId);
        Task<IEnumerable<MovieShowResponseDTO>> GetAvailableShowsAsync();
        Task<MovieShowResponseDTO> CreateMovieShowAsync(CreateMovieShowDTO dto);
        Task<bool> UpdateMovieShowAsync(int id, UpdateMovieShowDTO dto);
        Task<bool> DeleteMovieShowAsync(int id);
    }
}