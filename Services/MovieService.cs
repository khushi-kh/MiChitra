using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class MovieService : IMovieService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<MovieService> _logger;

        public MovieService(MiChitraDbContext context, ILogger<MovieService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _context.Movies.ToListAsync();
            var showStatuses = await GetMovieShowStatusesAsync(movies.Select(m => m.MovieId));
            return movies.Select(m => MapToResponseDto(m, showStatuses.GetValueOrDefault(m.MovieId)));
        }

        public async Task<MovieResponseDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return null;
            var showStatuses = await GetMovieShowStatusesAsync([id]);
            return MapToResponseDto(movie, showStatuses.GetValueOrDefault(id));
        }

        public async Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto)
        {
            var movie = new Movie
            {
                MovieName = dto.MovieName,
                Description = dto.Description,
                Language = dto.Language,
                Rating = dto.Rating
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Movie created with ID: {MovieId}", movie.MovieId);
            return MapToResponseDto(movie);
        }

        public async Task<bool> UpdateMovieAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return false;

            movie.MovieName = dto.MovieName;
            movie.Description = dto.Description;
            movie.Language = dto.Language;
            movie.Rating = dto.Rating;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Movie updated with ID: {MovieId}", id);
            return true;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return false;

            // Check if movie has future shows
            var hasActiveShows = await _context.MovieShows
                .AnyAsync(ms => ms.MovieId == id && ms.ShowTime > DateTime.UtcNow);
            
            if (hasActiveShows)
            {
                _logger.LogWarning("Cannot delete movie {MovieId} - has active shows", id);
                throw new InvalidOperationException("Cannot delete movie with active shows");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Movie deleted with ID: {MovieId}", id);
            return true;
        }

        public async Task<IEnumerable<MovieResponseDto>> GetMoviesByLanguageAsync(string language)
        {
            var movies = await _context.Movies
                .Where(m => m.Language.ToLower() == language.ToLower())
                .ToListAsync();
            var showStatuses = await GetMovieShowStatusesAsync(movies.Select(m => m.MovieId));
            return movies.Select(m => MapToResponseDto(m, showStatuses.GetValueOrDefault(m.MovieId)));
        }

        public async Task<IEnumerable<MovieResponseDto>> GetMoviesByRatingAsync(decimal minRating)
        {
            var movies = await _context.Movies
                .Where(m => m.Rating >= minRating)
                .ToListAsync();
            var showStatuses = await GetMovieShowStatusesAsync(movies.Select(m => m.MovieId));
            return movies.Select(m => MapToResponseDto(m, showStatuses.GetValueOrDefault(m.MovieId)));
        }

        public async Task<IEnumerable<MovieResponseDto>> SearchMovieAsync(string query)
        {
            var movies = await _context.Movies
                .Where(m => m.MovieName.Contains(query) || (m.Description != null && m.Description.Contains(query)))
                .ToListAsync();
            var showStatuses = await GetMovieShowStatusesAsync(movies.Select(m => m.MovieId));
            return movies.Select(m => MapToResponseDto(m, showStatuses.GetValueOrDefault(m.MovieId)));
        }

        private async Task<Dictionary<int, string>> GetMovieShowStatusesAsync(IEnumerable<int> movieIds)
        {
            var now = DateTime.UtcNow;
            var shows = await _context.MovieShows
                .Where(s => movieIds.Contains(s.MovieId) && s.ShowTime > now)
                .Select(s => new { s.MovieId, s.AvailableSeats })
                .ToListAsync();

            return shows
                .GroupBy(s => s.MovieId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        if (g.All(s => s.AvailableSeats == 0)) return "Sold Out";
                        if (g.Any(s => s.AvailableSeats <= 10)) return "Almost Full";
                        return "Available";
                    }
                );
        }

        private static MovieResponseDto MapToResponseDto(Movie movie, string? availabilityStatus = null)
        {
            return new MovieResponseDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                Description = movie.Description,
                Language = movie.Language,
                Rating = movie.Rating,
                AvailabilityStatus = availabilityStatus ?? "No Shows"
            };
        }
    }
}