using MiChitra.DTOs;
using MiChitra.Models;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IUnitOfWork unitOfWork, ILogger<MoviesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // 1. GET: api/movies
        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Fetching all movies");
            var movies = await _unitOfWork.Movies.GetAllAsync();
            var movieDtos = movies.Select(m => new MovieResponseDto
            {
                MovieId = m.MovieId,
                MovieName = m.MovieName,
                Description = m.Description,
                Language = m.Language,
                Rating = m.Rating
            });
            return Ok(movieDtos);
        }

        // 2. GET: api/movies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            _logger.LogInformation("Fetching movie with ID: {MovieId}", id);

            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
                return NotFound("Movie not found");
            }

            var movieDto = new MovieResponseDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                Description = movie.Description,
                Language = movie.Language,
                Rating = movie.Rating
            };

            return Ok(movieDto);
        }

        // 3. POST: api/movies
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto dto)
        {
            _logger.LogInformation("Adding a new movie: {MovieName}", dto.MovieName);

            var movie = new Movie
            {
                MovieName = dto.MovieName,
                Description = dto.Description,
                Language = dto.Language,
                Rating = dto.Rating
            };

            await _unitOfWork.Movies.AddAsync(movie);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Movie added successfully with ID: {MovieId}", movie.MovieId);

            return CreatedAtAction(nameof(GetMovieById), new { id = movie.MovieId }, movie);
        }

        // 4. PUT: api/movies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDto dto)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", id);

            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for update", id);
                return NotFound("Movie not found");
            }

            movie.MovieName = dto.MovieName;
            movie.Description = dto.Description;
            movie.Language = dto.Language;
            movie.Rating = dto.Rating;

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Movie with ID {MovieId} updated successfully", id);

            return NoContent();
        }

        // 5. DELETE: api/movies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _logger.LogInformation("Deleting movie with ID: {MovieId}", id);

            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for deletion", id);
                return NotFound("Movie not found");
            }

            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Movie with ID {MovieId} deleted successfully", id);

            return NoContent();
        }
    }
}
