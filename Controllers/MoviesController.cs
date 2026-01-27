using MiChitra.DTOs;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("GeneralPolicy")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Fetching all movies");
            var movies = await _movieService.GetAllMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            _logger.LogInformation("Fetching movie with ID: {MovieId}", id);
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
                return NotFound("Movie not found");
            }
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto dto)
        {
            _logger.LogInformation("Adding a new movie: {MovieName}", dto.MovieName);
            var movie = await _movieService.CreateMovieAsync(dto);
            _logger.LogInformation("Movie added successfully with ID: {MovieId}", movie.MovieId);
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.MovieId }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDto dto)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", id);
            var success = await _movieService.UpdateMovieAsync(id, dto);
            if (!success)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for update", id);
                return NotFound("Movie not found");
            }
            _logger.LogInformation("Movie with ID {MovieId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _logger.LogInformation("Deleting movie with ID: {MovieId}", id);
            var success = await _movieService.DeleteMovieAsync(id);
            if (!success)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for deletion", id);
                return NotFound("Movie not found");
            }
            _logger.LogInformation("Movie with ID {MovieId} deleted successfully", id);
            return NoContent();
        }
    }
}
