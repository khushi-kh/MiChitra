using Microsoft.EntityFrameworkCore;
using MiChitra.Models;
namespace MiChitra.Data
{
    public class MiChitraDbContext : DbContext
    {
        public MiChitraDbContext(DbContextOptions<MiChitraDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<MovieShow> MovieShows { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.ContactNumber).HasMaxLength(10);
                entity.Property(u => u.Role).HasConversion<int>();
            });

            // Movie entity configuration
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(m => m.MovieName).IsRequired();
                entity.Property(m => m.Description).HasMaxLength(1000);
                entity.Property(m => m.Language).IsRequired();
                entity.Property(m => m.Rating).HasPrecision(2, 1);
            });

            // Theatre entity configuration
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.Property(t => t.Name).IsRequired();
                entity.Property(t => t.City).IsRequired();
            });

            // MovieShow entity configuration
            modelBuilder.Entity<MovieShow>(entity =>
            {
                entity.HasOne(ms => ms.Movie)
                      .WithMany(m => m.MovieShows)
                      .HasForeignKey(ms => ms.MovieId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ms => ms.Theatre)
                      .WithMany(t => t.MovieShows)
                      .HasForeignKey(ms => ms.TheatreId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(ms => ms.Status).HasConversion<int>();
            });

            // Ticket entity configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tickets)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.MovieShow)
                      .WithMany(ms => ms.Tickets)   
                      .HasForeignKey(t => t.MovieShowId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(t => t.TotalPrice).HasPrecision(10, 2);
                entity.Property(t => t.Status).HasConversion<int>();
            });

        }
    }
}