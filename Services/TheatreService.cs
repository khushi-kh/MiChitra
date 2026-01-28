using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<TheatreService> _logger;

        public TheatreService(MiChitraDbContext context, ILogger<TheatreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TheatreResponseDTO>> GetAllTheatresAsync()
        {
            var theatres = await _context.Theatres.ToListAsync();
            return theatres.Select(MapToResponseDto);
        }

        public async Task<TheatreResponseDTO?> GetTheatreByIdAsync(int id)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            return theatre == null ? null : MapToResponseDto(theatre);
        }

        public async Task<TheatreResponseDTO> CreateTheatreAsync(CreateTheatreDTO dto)
        {
            var theatre = new Theatre
            {
                Name = dto.Name,
                City = dto.City,
                isActive = true
            };

            _context.Theatres.Add(theatre);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Theatre created with ID: {TheatreId}", theatre.TheatreId);
            return MapToResponseDto(theatre);
        }

        public async Task<bool> UpdateTheatreAsync(int id, UpdateTheatreDTO dto)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre == null) return false;

            theatre.Name = dto.Name;
            theatre.City = dto.City;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Theatre updated with ID: {TheatreId}", id);
            return true;
        }

        public async Task<bool> DeactivateTheatreAsync(int id)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre == null) return false;

            // Check if theatre has future shows
            var hasActiveShows = await _context.MovieShows
                .AnyAsync(ms => ms.TheatreId == id && ms.ShowTime > DateTime.UtcNow);
            
            if (hasActiveShows)
            {
                _logger.LogWarning("Cannot deactivate theatre {TheatreId} - has active shows", id);
                throw new InvalidOperationException("Cannot deactivate theatre with active shows");
            }

            theatre.isActive = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Theatre deactivated with ID: {TheatreId}", id);
            return true;
        }

        public async Task<IEnumerable<TheatreResponseDTO>> GetTheatresByCityAsync(string city)
        {
            var theatres = await _context.Theatres
                .Where(t => t.City.ToLower() == city.ToLower())
                .ToListAsync();
            return theatres.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<TheatreResponseDTO>> GetActiveTheatresAsync()
        {
            var theatres = await _context.Theatres
                .Where(t => t.isActive)
                .ToListAsync();
            return theatres.Select(MapToResponseDto);
        }

        private static TheatreResponseDTO MapToResponseDto(Theatre theatre)
        {
            return new TheatreResponseDTO
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                City = theatre.City,
                IsActive = theatre.isActive
            };
        }
    }
}