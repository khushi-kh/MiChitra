using MiChitra.DTOs;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllMovieShows()
        {
            var shows = await _movieShowService.GetAllMovieShowsAsync();
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieShowById(int id)
        {
            var show = await _movieShowService.GetMovieShowByIdAsync(id);
            if (show == null)
                return NotFound("Movie show not found");
            return Ok(show);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            var shows = await _movieShowService.GetShowsByMovieIdAsync(movieId);
            return Ok(shows);
        }

        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByTheatre(int theatreId)
        {
            var shows = await _movieShowService.GetShowsByTheatreIdAsync(theatreId);
            return Ok(shows);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableShows()
        {
            var shows = await _movieShowService.GetAvailableShowsAsync();
            return Ok(shows);
        }

        [HttpGet("movie/{movieId}/theatres")]
        public async Task<IActionResult> GetTheatresByMovie(int movieId)
        {
            var shows = await _movieShowService.GetShowsByMovieIdAsync(movieId);
            var theatres = shows.Select(s => new { s.TheatreId, s.TheatreName, s.City }).Distinct().ToList();
            return Ok(theatres);
        }

        [HttpGet("movie/{movieId}/theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByMovieAndTheatre(int movieId, int theatreId)
        {
            var shows = await _movieShowService.GetShowsByMovieIdAsync(movieId);
            var filteredShows = shows.Where(s => s.TheatreId == theatreId).ToList();
            return Ok(filteredShows);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMovieShow([FromBody] CreateMovieShowDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var show = await _movieShowService.CreateMovieShowAsync(dto);
                return CreatedAtAction(nameof(GetMovieShowById), new { id = show.Id }, show);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovieShow(int id, [FromBody] UpdateMovieShowDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _movieShowService.UpdateMovieShowAsync(id, dto);
                if (!success)
                    return NotFound("Movie show not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieShow(int id)
        {
            try
            {
                var success = await _movieShowService.DeleteMovieShowAsync(id);
                if (!success)
                    return NotFound("Movie show not found");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}