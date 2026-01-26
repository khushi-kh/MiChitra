using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("GeneralPolicy")]
    public class MoviesController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MiChitraDbContext context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. GET: api/movies
        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            _logger.LogInformation("Fetching all movies");
            var movies = await _context.Movies
                .Select(m => new MovieResponseDto
                {
                    MovieId = m.MovieId,
                    MovieName = m.MovieName,
                    Description = m.Description,
                    Language = m.Language,
                    Rating = m.Rating
                })
                .ToListAsync();
            return Ok(movies);
        }

        // 2. GET: api/movies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            _logger.LogInformation("Fetching movie with ID: {MovieId}", id);

            var movie = await _context.Movies
                .Where(m => m.MovieId == id)
                .Select(m => new MovieResponseDto
                {
                    MovieId = m.MovieId,
                    MovieName = m.MovieName,
                    Description = m.Description,
                    Language = m.Language,
                    Rating = m.Rating
                })
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found", id);
                return NotFound("Movie not found");
            }

            return Ok(movie);
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

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Movie added successfully with ID: {MovieId}", movie.MovieId);

            return CreatedAtAction(nameof(GetMovieById), new { id = movie.MovieId }, movie);
        }

        // 4. PUT: api/movies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDto dto)
        {
            _logger.LogInformation("Updating movie with ID: {MovieId}", id);

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for update", id);
                return NotFound("Movie not found");
            }

            movie.MovieName = dto.MovieName;
            movie.Description = dto.Description;
            movie.Language = dto.Language;
            movie.Rating = dto.Rating;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Movie with ID {MovieId} updated successfully", id);

            return NoContent();
        }

        // 5. DELETE: api/movies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            _logger.LogInformation("Deleting movie with ID: {MovieId}", id);

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID {MovieId} not found for deletion", id);
                return NotFound("Movie not found");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Movie with ID {MovieId} deleted successfully", id);

            return NoContent();
        }
    }
}
