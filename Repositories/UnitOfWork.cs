using Microsoft.EntityFrameworkCore.Storage;
using MiChitra.Data;
using MiChitra.Interfaces;

namespace MiChitra.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MiChitraDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(MiChitraDbContext context)
        {
            _context = context;
            Movies = new MovieRepository(_context);
            Theatres = new TheatreRepository(_context);
            MovieShows = new MovieShowRepository(_context);
            Tickets = new TicketRepository(_context);
            Users = new UserRepository(_context);
        }

        public IMovieRepository Movies { get; }
        public ITheatreRepository Theatres { get; }
        public IMovieShowRepository MovieShows { get; }
        public ITicketRepository Tickets { get; }
        public IUserRepository Users { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}