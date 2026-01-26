using MiChitra.Models;

namespace MiChitra.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetMoviesByLanguageAsync(string language);
        Task<IEnumerable<Movie>> GetMoviesByRatingAsync(decimal minRating);
    }

    public interface ITheatreRepository : IRepository<Theatre>
    {
        Task<IEnumerable<Theatre>> GetTheatresByCityAsync(string city);
        Task<IEnumerable<Theatre>> GetActiveTheatresAsync();
    }

    public interface IMovieShowRepository : IRepository<MovieShow>
    {
        Task<IEnumerable<MovieShow>> GetShowsByMovieIdAsync(int movieId);
        Task<IEnumerable<MovieShow>> GetShowsByTheatreIdAsync(int theatreId);
        Task<IEnumerable<MovieShow>> GetAvailableShowsAsync();
        Task<MovieShow?> GetShowWithDetailsAsync(int showId);
    }

    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(int userId);
        Task<IEnumerable<Ticket>> GetTicketsByShowIdAsync(int showId);
        Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
}