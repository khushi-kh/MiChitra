namespace MiChitra.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        ITheatreRepository Theatres { get; }
        IMovieShowRepository MovieShows { get; }
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}