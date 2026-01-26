using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(MiChitraDbContext context) : base(context) { }

        public async Task<IEnumerable<Movie>> GetMoviesByLanguageAsync(string language)
        {
            return await _dbSet.Where(m => m.Language == language).ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByRatingAsync(decimal minRating)
        {
            return await _dbSet.Where(m => m.Rating >= minRating).ToListAsync();
        }
    }

    public class TheatreRepository : Repository<Theatre>, ITheatreRepository
    {
        public TheatreRepository(MiChitraDbContext context) : base(context) { }

        public async Task<IEnumerable<Theatre>> GetTheatresByCityAsync(string city)
        {
            return await _dbSet.Where(t => t.City.ToLower() == city.ToLower()).ToListAsync();
        }

        public async Task<IEnumerable<Theatre>> GetActiveTheatresAsync()
        {
            return await _dbSet.Where(t => t.isActive).ToListAsync();
        }
    }

    public class MovieShowRepository : Repository<MovieShow>, IMovieShowRepository
    {
        public MovieShowRepository(MiChitraDbContext context) : base(context) { }

        public async Task<IEnumerable<MovieShow>> GetShowsByMovieIdAsync(int movieId)
        {
            return await _dbSet.Where(ms => ms.MovieId == movieId).ToListAsync();
        }

        public async Task<IEnumerable<MovieShow>> GetShowsByTheatreIdAsync(int theatreId)
        {
            return await _dbSet.Where(ms => ms.TheatreId == theatreId).ToListAsync();
        }

        public async Task<IEnumerable<MovieShow>> GetAvailableShowsAsync()
        {
            return await _dbSet.Where(ms => ms.Status == MovieShowStatus.Available).ToListAsync();
        }

        public async Task<MovieShow?> GetShowWithDetailsAsync(int showId)
        {
            return await _dbSet.Include(ms => ms.Movie).Include(ms => ms.Theatre).FirstOrDefaultAsync(ms => ms.Id == showId);
        }
    }

    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(MiChitraDbContext context) : base(context) { }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(t => t.UserId == userId).Include(t => t.MovieShow).ThenInclude(ms => ms.Movie).ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByShowIdAsync(int showId)
        {
            return await _dbSet.Where(t => t.MovieShowId == showId).Include(t => t.User).ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
        {
            return await _dbSet.Where(t => t.Status == status).Include(t => t.User).Include(t => t.MovieShow).ToListAsync();
        }
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MiChitraDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet.Where(u => u.IsActive).ToListAsync();
        }
    }
}