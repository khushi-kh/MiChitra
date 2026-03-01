using Microsoft.AspNetCore.Mvc;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;
        private readonly ILogger<TheatresController> _logger;

        public TheatresController(ITheatreService theatreService, ILogger<TheatresController> logger)
        {
            _theatreService = theatreService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTheatres()
        {
            _logger.LogInformation("Fetching all theatres");
            var theatres = await _theatreService.GetAllTheatresAsync();
            return Ok(theatres);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTheatreById(int id)
        {
            _logger.LogInformation("Fetching theatre with ID: {TheatreId}", id);
            var theatre = await _theatreService.GetTheatreByIdAsync(id);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found", id);
                return NotFound("Theatre not found");
            }
            return Ok(theatre);
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetTheatresByCity(string city)
        {
            _logger.LogInformation("Fetching theatres in city: {City}", city);
            var theatres = await _theatreService.GetTheatresByCityAsync(city);
            return Ok(theatres);
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetAllCities()
        {
            _logger.LogInformation("Fetching all cities");
            var cities = await _theatreService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddTheatre([FromBody] CreateTheatreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            _logger.LogInformation("Adding a new theatre: {TheatreName}", dto.Name);
            var theatre = await _theatreService.CreateTheatreAsync(dto);
            return CreatedAtAction(nameof(GetTheatreById), new { id = theatre.TheatreId }, theatre);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheatre(int id, [FromBody] UpdateTheatreDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var success = await _theatreService.UpdateTheatreAsync(id, dto);
            if (!success)
            {
                _logger.LogWarning("Theatre with ID {TheatreId} not found for update", id);
                return NotFound("Theatre not found");
            }
            _logger.LogInformation("Theatre with ID {TheatreId} updated successfully", id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> InActiveTheatre(int id)
        {
            try
            {
                var success = await _theatreService.DeactivateTheatreAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Theatre with ID {TheatreId} not found for deactivation", id);
                    return NotFound("Theatre not found");
                }
                _logger.LogInformation("Theatre with ID {TheatreId} deactivated successfully", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
