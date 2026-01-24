using MiChitra.Data;
using MiChitra.Models;
using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieShowsController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<MovieShowsController> _logger;

        public MovieShowsController(MiChitraDbContext context, ILogger<MovieShowsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/movieshows
        [HttpGet]
        public async Task<IActionResult> GetAllShows()
        {
            var shows = await _context.MovieShows
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .ToListAsync();

            return Ok(shows);
        }

        // GET: api/movieshows/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowById(int id)
        {
            var show = await _context.MovieShows
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
                return NotFound("Show not found");

            return Ok(show);
        }

        // GET: api/movieshows/movie/{movieId}
        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            var shows = await _context.MovieShows
                .Include(s => s.Theatre)
                .Where(s => s.MovieId == movieId)
                .ToListAsync();

            return Ok(shows);
        }

        // GET: api/movieshows/theatre/{theatreId}
        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByTheatre(int theatreId)
        {
            var shows = await _context.MovieShows
                .Include(s => s.Movie)
                .Where(s => s.TheatreId == theatreId)
                .ToListAsync();

            return Ok(shows);
        }

        // GET: api/movieshows/city/{city}
        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetShowsByCity(string city)
        {
            var shows = await _context.MovieShows
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.Theatre!.City.ToLower() == city.ToLower())
                .ToListAsync();

            return Ok(shows);
        }

        // POST: api/movieshows
        [HttpPost]
        public async Task<IActionResult> CreateShow([FromBody] CreateMovieShowDto dto)
        {
            var show = new MovieShow
            {
                MovieId = dto.MovieId,
                TheatreId = dto.TheatreId,
                ShowTime = dto.ShowTime,
                TotalSeats = dto.TotalSeats,
                AvailableSeats = dto.TotalSeats,
                PricePerSeat = dto.PricePerSeat,
                Status = MovieShowStatus.Available
            };

            _context.MovieShows.Add(show);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShowById), new { id = show.Id }, show);
        }

        // PUT: api/movieshows/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] UpdateMovieShowDto dto)
        {
            var show = await _context.MovieShows.FindAsync(id);
            if (show == null)
                return NotFound("Show not found");

            show.ShowTime = dto.ShowTime;
            show.TotalSeats = dto.TotalSeats;
            show.AvailableSeats = dto.AvailableSeats;
            show.PricePerSeat = dto.PricePerSeat;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/movieshows/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var show = await _context.MovieShows.FindAsync(id);
            if (show == null)
                return NotFound("Show not found");

            _context.MovieShows.Remove(show);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
