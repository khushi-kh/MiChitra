using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MovieResponseDto>> GetAllMoviesAsync()
        {
            var movies = await _unitOfWork.Movies.GetAllAsync();
            return movies.Select(m => new MovieResponseDto
            {
                MovieId = m.MovieId,
                MovieName = m.MovieName,
                Description = m.Description,
                Language = m.Language,
                Rating = m.Rating
            });
        }

        public async Task<MovieResponseDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null) return null;

            return new MovieResponseDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                Description = movie.Description,
                Language = movie.Language,
                Rating = movie.Rating
            };
        }

        public async Task<MovieResponseDto> CreateMovieAsync(CreateMovieDto dto)
        {
            var movie = new Movie
            {
                MovieName = dto.MovieName,
                Description = dto.Description,
                Language = dto.Language,
                Rating = dto.Rating
            };

            await _unitOfWork.Movies.AddAsync(movie);
            await _unitOfWork.SaveChangesAsync();

            return new MovieResponseDto
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                Description = movie.Description,
                Language = movie.Language,
                Rating = movie.Rating
            };
        }

        public async Task<bool> UpdateMovieAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null) return false;

            movie.MovieName = dto.MovieName;
            movie.Description = dto.Description;
            movie.Language = dto.Language;
            movie.Rating = dto.Rating;

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id);
            if (movie == null) return false;

            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}