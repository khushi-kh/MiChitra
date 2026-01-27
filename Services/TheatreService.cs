using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TheatreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TheatreResponseDTO>> GetAllTheatresAsync()
        {
            var theatres = await _unitOfWork.Theatres.GetAllAsync();
            return theatres.Select(t => new TheatreResponseDTO
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                City = t.City,
                IsActive = t.isActive
            });
        }

        public async Task<TheatreResponseDTO?> GetTheatreByIdAsync(int id)
        {
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null) return null;

            return new TheatreResponseDTO
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                City = theatre.City,
                IsActive = theatre.isActive
            };
        }

        public async Task<IEnumerable<TheatreResponseDTO>> GetTheatresByCityAsync(string city)
        {
            var theatres = await _unitOfWork.Theatres.GetTheatresByCityAsync(city);
            return theatres.Select(t => new TheatreResponseDTO
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                City = t.City,
                IsActive = t.isActive
            });
        }

        public async Task<TheatreResponseDTO> CreateTheatreAsync(CreateTheatreDTO dto)
        {
            var theatre = new Theatre
            {
                Name = dto.Name,
                City = dto.City
            };

            await _unitOfWork.Theatres.AddAsync(theatre);
            await _unitOfWork.SaveChangesAsync();

            return new TheatreResponseDTO
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                City = theatre.City,
                IsActive = theatre.isActive
            };
        }

        public async Task<bool> UpdateTheatreAsync(int id, UpdateTheatreDTO dto)
        {
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null) return false;

            theatre.Name = dto.Name;
            theatre.City = dto.City;

            _unitOfWork.Theatres.Update(theatre);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateTheatreAsync(int id)
        {
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null) return false;

            theatre.isActive = false;
            _unitOfWork.Theatres.Update(theatre);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}