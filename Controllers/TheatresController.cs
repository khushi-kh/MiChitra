using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using MiChitra.DTOs;
using MiChitra.Interfaces;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TheatresController> _logger;
        public TheatresController(IUnitOfWork unitOfWork, ILogger<TheatresController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTheatres()
        {
            _logger.LogInformation("Fetching all theatres");
            var theatres = await _unitOfWork.Theatres.GetAllAsync();
            var theatreDtos = theatres.Select(t => new TheatreResponseDTO
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                City = t.City,
                IsActive = t.isActive
            });
            return Ok(theatreDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTheatreById(int id)
        {
            _logger.LogInformation("Fetching theatre with ID: {TheatreId}", id);
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found", id);
                return NotFound("Theatre not found");
            }
            var theatreDto = new TheatreResponseDTO
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                City = theatre.City,
                IsActive = theatre.isActive
            };
            return Ok(theatreDto);
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetTheatresByCity(string city)
        {
            _logger.LogInformation("Fetching theatres in city: {City}", city);
            var theatres = await _unitOfWork.Theatres.GetTheatresByCityAsync(city);
            var theatreDtos = theatres.Select(t => new TheatreResponseDTO
            {
                TheatreId = t.TheatreId,
                Name = t.Name,
                City = t.City,
                IsActive = t.isActive
            });
            return Ok(theatreDtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddTheatre([FromBody] CreateTheatreDTO dto)
        {
            _logger.LogInformation("Adding a new theatre: {TheatreName}", dto.Name);
            var theatre = new Theatre
            {
                Name = dto.Name,
                City = dto.City
            };
            await _unitOfWork.Theatres.AddAsync(theatre);
            await _unitOfWork.SaveChangesAsync();
            var theatreDto = new TheatreResponseDTO
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                City = theatre.City,
                IsActive = theatre.isActive
            };
            return CreatedAtAction(nameof(GetTheatreById), new { id = theatre.TheatreId }, theatreDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheatre(int id, [FromBody] UpdateTheatreDTO dto)
        {
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found for update", id);
                return NotFound("Theatre not found");
            }
            theatre.Name = dto.Name;
            theatre.City = dto.City;
            _unitOfWork.Theatres.Update(theatre);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Theatre with ID {TheatreId} updated successfully", id);
            return NoContent();
        }

        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> InActiveTheatre(int id)
        {
            var theatre = await _unitOfWork.Theatres.GetByIdAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found for deactivation", id);
                return NotFound("Theatre not found");
            }
            theatre.isActive = false;
            _unitOfWork.Theatres.Update(theatre);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Theatre with ID {TheatreId} deactivated successfully", id);
            return NoContent();
        }
    }
}
