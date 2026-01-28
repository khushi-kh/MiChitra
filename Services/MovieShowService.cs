using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class MovieShowService : IMovieShowService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<MovieShowService> _logger;

        public MovieShowService(MiChitraDbContext context, ILogger<MovieShowService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetAllMovieShowsAsync()
        {
            var shows = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .ToListAsync();
            return shows.Select(MapToResponseDto);
        }

        public async Task<MovieShowResponseDTO?> GetMovieShowByIdAsync(int id)
        {
            var show = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .FirstOrDefaultAsync(ms => ms.Id == id);
            return show == null ? null : MapToResponseDto(show);
        }

        public async Task<MovieShowResponseDTO> CreateMovieShowAsync(CreateMovieShowDTO dto)
        {
            // Validate movie and theatre exist
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId);
            var theatreExists = await _context.Theatres.AnyAsync(t => t.TheatreId == dto.TheatreId && t.isActive);

            if (!movieExists)
                throw new InvalidOperationException("Movie not found");
            if (!theatreExists)
                throw new InvalidOperationException("Theatre not found or inactive");

            var show = new MovieShow
            {
                MovieId = dto.MovieId,
                TheatreId = dto.TheatreId,
                ShowTime = dto.ShowTime,
                TotalSeats = dto.TotalSeats,
                AvailableSeats = dto.TotalSeats,
                PricePerSeat = dto.PricePerSeat,
                Status = MovieShowStatus.Available
            };

            _context.MovieShows.Add(show);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Movie show created with ID: {ShowId}", show.Id);
            
            // Reload with includes
            var createdShow = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .FirstAsync(ms => ms.Id == show.Id);

            return MapToResponseDto(createdShow);
        }

        public async Task<bool> UpdateMovieShowAsync(int id, UpdateMovieShowDTO dto)
        {
            var show = await _context.MovieShows.FindAsync(id);
            if (show == null) return false;

            if (show.ShowTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot update past shows");

            show.ShowTime = dto.ShowTime;
            show.PricePerSeat = dto.PricePerSeat;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Movie show updated with ID: {ShowId}", id);
            return true;
        }

        public async Task<bool> DeleteMovieShowAsync(int id)
        {
            var show = await _context.MovieShows.FindAsync(id);
            if (show == null) return false;

            if (show.ShowTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot delete past shows");

            var hasBookings = await _context.Tickets.AnyAsync(t => t.MovieShowId == id);
            if (hasBookings)
                throw new InvalidOperationException("Cannot delete show with existing bookings");

            _context.MovieShows.Remove(show);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Movie show deleted with ID: {ShowId}", id);
            return true;
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetShowsByMovieIdAsync(int movieId)
        {
            var shows = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .Where(ms => ms.MovieId == movieId && ms.ShowTime > DateTime.UtcNow)
                .OrderBy(ms => ms.ShowTime)
                .ToListAsync();
            return shows.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetShowsByTheatreIdAsync(int theatreId)
        {
            var shows = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .Where(ms => ms.TheatreId == theatreId && ms.ShowTime > DateTime.UtcNow)
                .OrderBy(ms => ms.ShowTime)
                .ToListAsync();
            return shows.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetAvailableShowsAsync()
        {
            var shows = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .Where(ms => ms.Status == MovieShowStatus.Available && ms.ShowTime > DateTime.UtcNow)
                .OrderBy(ms => ms.ShowTime)
                .ToListAsync();
            return shows.Select(MapToResponseDto);
        }

        private static MovieShowResponseDTO MapToResponseDto(MovieShow show)
        {
            return new MovieShowResponseDTO
            {
                Id = show.Id,
                MovieId = show.MovieId,
                TheatreId = show.TheatreId,
                ShowTime = show.ShowTime,
                TotalSeats = show.TotalSeats,
                AvailableSeats = show.AvailableSeats,
                PricePerSeat = show.PricePerSeat,
                Status = show.Status
            };
        }
    }
}