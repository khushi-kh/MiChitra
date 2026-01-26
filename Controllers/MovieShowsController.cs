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
                .Select(s => new MovieShowResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie!.MovieName,
                    TheatreId = s.TheatreId,
                    TheatreName = s.Theatre!.Name,
                    City = s.Theatre!.City,
                    ShowTime = s.ShowTime,
                    TotalSeats = s.TotalSeats,
                    AvailableSeats = s.AvailableSeats,
                    PricePerSeat = s.PricePerSeat,
                    Status = s.Status
                })
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
                .Where(s => s.Id == id)
                .Select(s => new MovieShowResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie!.MovieName,
                    TheatreId = s.TheatreId,
                    TheatreName = s.Theatre!.Name,
                    City = s.Theatre!.City,
                    ShowTime = s.ShowTime,
                    TotalSeats = s.TotalSeats,
                    AvailableSeats = s.AvailableSeats,
                    PricePerSeat = s.PricePerSeat,
                    Status = s.Status
                })
                .FirstOrDefaultAsync();

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
                .Select(s => new MovieShowResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie!.MovieName,
                    TheatreId = s.TheatreId,
                    TheatreName = s.Theatre!.Name,
                    City = s.Theatre!.City,
                    ShowTime = s.ShowTime,
                    TotalSeats = s.TotalSeats,
                    AvailableSeats = s.AvailableSeats,
                    PricePerSeat = s.PricePerSeat,
                    Status = s.Status
                })
                .ToListAsync();

            return Ok(shows);
        }

        // GET: api/movieshows/theatre/{theatreId}
        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByTheatre(int theatreId)
        {
            var shows = await _context.MovieShows
                .Include(s => s.Movie)
                .Include(s => s.Theatre)
                .Where(s => s.TheatreId == theatreId)
                .Select(s => new MovieShowResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie!.MovieName,
                    TheatreId = s.TheatreId,
                    TheatreName = s.Theatre!.Name,
                    City = s.Theatre!.City,
                    ShowTime = s.ShowTime,
                    TotalSeats = s.TotalSeats,
                    AvailableSeats = s.AvailableSeats,
                    PricePerSeat = s.PricePerSeat,
                    Status = s.Status
                })
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
                .Select(s => new MovieShowResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    MovieName = s.Movie!.MovieName,
                    TheatreId = s.TheatreId,
                    TheatreName = s.Theatre!.Name,
                    City = s.Theatre!.City,
                    ShowTime = s.ShowTime,
                    TotalSeats = s.TotalSeats,
                    AvailableSeats = s.AvailableSeats,
                    PricePerSeat = s.PricePerSeat,
                    Status = s.Status
                })
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

            // Fetch again and project into DTO (this is where names get populated)
            var result = await _context.MovieShows
                .Where(ms => ms.Id == show.Id)
                .Select(ms => new MovieShowResponseDTO
                {
                    Id = ms.Id,

                    MovieId = ms.MovieId,
                    MovieName = ms.Movie!.MovieName,

                    TheatreId = ms.TheatreId,
                    TheatreName = ms.Theatre!.Name,

                    ShowTime = ms.ShowTime,
                    TotalSeats = ms.TotalSeats,
                    AvailableSeats = ms.AvailableSeats,
                    PricePerSeat = ms.PricePerSeat,
                    Status = MovieShowStatus.Available


                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetShowById), new { id = result.Id }, result);
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