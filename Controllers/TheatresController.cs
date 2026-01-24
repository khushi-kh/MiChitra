using MiChitra.Data;
using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiChitra.DTOs;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<TheatresController> _logger;
        public TheatresController(MiChitraDbContext context, ILogger<TheatresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/theatres
        [HttpGet]
        public async Task<IActionResult> GetAllTheatres()
        {
            _logger.LogInformation("Fetching all theatres");
            var theatres = await _context.Theatres.ToListAsync();
            return Ok(theatres);
        }

        // GET: api/theatres/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTheatreById(int id)
        {
            _logger.LogInformation("Fetching theatre with ID: {TheatreId}", id);
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found", id);
                return NotFound("Theatre not found");
            }
            return Ok(theatre);
        }

        // GET: api/theatres/city/{city}
        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetTheatresByCity(string city)
        {
            _logger.LogInformation("Fetching theatres in city: {City}", city);
            var theatres = await _context.Theatres
                .Where(t => t.City.ToLower() == city.ToLower())
                .ToListAsync();
            return Ok(theatres);
        }

        // POST: api/theatres
        [HttpPost]
        public async Task<IActionResult> AddTheatre([FromBody] CreateTheatreDTO dto)
        {
            _logger.LogInformation("Adding a new theatre: {TheatreName}", dto.Name);
            var theatre = new Theatre
            {
                Name = dto.Name,
                City = dto.City
            };
            _context.Theatres.Add(theatre);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTheatreById), new { id = theatre.TheatreId }, theatre);

        }

        // PUT: api/theatres/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheatre(int id, [FromBody] UpdateTheatreDTO dto)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found for update", id);
                return NotFound("Theatre not found");
            }
            theatre.Name = dto.Name;
            theatre.City = dto.City;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Theatre with ID {TheatreId} updated successfully", id);
            return NoContent();
        }

        // PUT: api/theatres/deactivate/{id}
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> InActiveTheatre(int id)
        {
            var theatre = await _context.Theatres.FindAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found for deactivation", id);
                return NotFound("Theatre not found");
            }
            theatre.isActive = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Theatre with ID {TheatreId} deactivated successfully", id);
            return NoContent();
        }
    }
}
