using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using MiChitra.Interfaces;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieShowsController : ControllerBase
    {
        private readonly IMovieShowService _movieShowService;
        private readonly ILogger<MovieShowsController> _logger;

        public MovieShowsController(IMovieShowService movieShowService, ILogger<MovieShowsController> logger)
        {
            _movieShowService = movieShowService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShows()
        {
            var shows = await _movieShowService.GetAllShowsAsync();
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowById(int id)
        {
            var show = await _movieShowService.GetShowByIdAsync(id);
            if (show == null)
                return NotFound("Show not found");
            return Ok(show);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            var shows = await _movieShowService.GetShowsByMovieAsync(movieId);
            return Ok(shows);
        }

        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByTheatre(int theatreId)
        {
            var shows = await _movieShowService.GetShowsByTheatreAsync(theatreId);
            return Ok(shows);
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetShowsByCity(string city)
        {
            var shows = await _movieShowService.GetShowsByCityAsync(city);
            return Ok(shows);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShow([FromBody] CreateMovieShowDto dto)
        {
            var show = await _movieShowService.CreateShowAsync(dto);
            return CreatedAtAction(nameof(GetShowById), new { id = show.Id }, show);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] UpdateMovieShowDto dto)
        {
            var success = await _movieShowService.UpdateShowAsync(id, dto);
            if (!success)
                return NotFound("Show not found");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var success = await _movieShowService.DeleteShowAsync(id);
            if (!success)
                return NotFound("Show not found");
            return NoContent();
        }
    }
}