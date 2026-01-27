using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class MovieShowService : IMovieShowService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieShowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetAllShowsAsync()
        {
            var shows = await _unitOfWork.MovieShows.GetAllAsync();
            return shows.Select(s => new MovieShowResponseDTO
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
        }

        public async Task<MovieShowResponseDTO?> GetShowByIdAsync(int id)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null) return null;

            return new MovieShowResponseDTO
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
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetShowsByMovieAsync(int movieId)
        {
            var shows = await _unitOfWork.MovieShows.GetShowsByMovieIdAsync(movieId);
            return shows.Select(s => new MovieShowResponseDTO
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
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetShowsByTheatreAsync(int theatreId)
        {
            var shows = await _unitOfWork.MovieShows.GetShowsByTheatreIdAsync(theatreId);
            return shows.Select(s => new MovieShowResponseDTO
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
        }

        public async Task<IEnumerable<MovieShowResponseDTO>> GetShowsByCityAsync(string city)
        {
            var theatres = await _unitOfWork.Theatres.GetTheatresByCityAsync(city);
            var shows = new List<MovieShow>();
            foreach (var theatre in theatres)
            {
                var theatreShows = await _unitOfWork.MovieShows.GetShowsByTheatreIdAsync(theatre.TheatreId);
                shows.AddRange(theatreShows);
            }
            return shows.Select(s => new MovieShowResponseDTO
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
        }

        public async Task<MovieShowResponseDTO> CreateShowAsync(CreateMovieShowDto dto)
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

            return new MovieShowResponseDTO
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
        }

        public async Task<bool> UpdateShowAsync(int id, UpdateMovieShowDto dto)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null) return false;

            show.ShowTime = dto.ShowTime;
            show.TotalSeats = dto.TotalSeats;
            show.AvailableSeats = dto.AvailableSeats;
            show.PricePerSeat = dto.PricePerSeat;

            _unitOfWork.MovieShows.Update(show);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShowAsync(int id)
        {
            var show = await _unitOfWork.MovieShows.GetByIdAsync(id);
            if (show == null) return false;

            _unitOfWork.MovieShows.Delete(show);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}