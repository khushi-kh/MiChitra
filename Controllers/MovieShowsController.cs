using MiChitra.Models;
using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using MiChitra.Interfaces;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieShowsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MovieShowsController> _logger;

        public MovieShowsController(IUnitOfWork unitOfWork, ILogger<MovieShowsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShows()
        {
            var shows = await _unitOfWork.MovieShows.GetAllAsync();
            var showDtos = shows.Select(s => new MovieShowResponseDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                TheatreId = s.TheatreId,
                ShowTime = s.ShowTime,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.AvailableSeats,
                PricePerSeat = s.PricePerSeat,
                Status = s.Status
            });
            return Ok(showDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowById(int id)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null)
                return NotFound("Show not found");

            var showDto = new MovieShowResponseDTO
            {
                Id = show.Id,
                MovieId = show.MovieId,
                TheatreId = show.TheatreId,
                ShowTime = show.ShowTime,
                TotalSeats = show.TotalSeats,
                AvailableSeats = show.AvailableSeats,
                PricePerSeat = show.PricePerSeat,
                Status = show.Status
            };
            return Ok(showDto);
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetShowsByMovie(int movieId)
        {
            var shows = await _unitOfWork.MovieShows.GetShowsByMovieIdAsync(movieId);
            var showDtos = shows.Select(s => new MovieShowResponseDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                TheatreId = s.TheatreId,
                ShowTime = s.ShowTime,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.AvailableSeats,
                PricePerSeat = s.PricePerSeat,
                Status = s.Status
            });
            return Ok(showDtos);
        }

        [HttpGet("theatre/{theatreId}")]
        public async Task<IActionResult> GetShowsByTheatre(int theatreId)
        {
            var shows = await _unitOfWork.MovieShows.GetShowsByTheatreIdAsync(theatreId);
            var showDtos = shows.Select(s => new MovieShowResponseDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                TheatreId = s.TheatreId,
                ShowTime = s.ShowTime,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.AvailableSeats,
                PricePerSeat = s.PricePerSeat,
                Status = s.Status
            });
            return Ok(showDtos);
        }

        [HttpGet("city/{city}")]
        public async Task<IActionResult> GetShowsByCity(string city)
        {
            var theatres = await _unitOfWork.Theatres.GetTheatresByCityAsync(city);
            var shows = new List<MovieShow>();
            foreach (var theatre in theatres)
            {
                var theatreShows = await _unitOfWork.MovieShows.GetShowsByTheatreIdAsync(theatre.TheatreId);
                shows.AddRange(theatreShows);
            }
            var showDtos = shows.Select(s => new MovieShowResponseDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                TheatreId = s.TheatreId,
                ShowTime = s.ShowTime,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.AvailableSeats,
                PricePerSeat = s.PricePerSeat,
                Status = s.Status
            });
            return Ok(showDtos);
        }

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

            await _unitOfWork.MovieShows.AddAsync(show);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShowById), new { id = show.Id }, show);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShow(int id, [FromBody] UpdateMovieShowDto dto)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null)
                return NotFound("Show not found");

            show.ShowTime = dto.ShowTime;
            show.TotalSeats = dto.TotalSeats;
            show.AvailableSeats = dto.AvailableSeats;
            show.PricePerSeat = dto.PricePerSeat;

            _unitOfWork.MovieShows.Update(show);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(int id)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null)
                return NotFound("Show not found");

            _unitOfWork.MovieShows.Delete(show);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}