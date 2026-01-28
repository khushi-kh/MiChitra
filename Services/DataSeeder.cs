using MiChitra.Data;
using MiChitra.Models;
using Microsoft.EntityFrameworkCore;

namespace MiChitra.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MiChitraDbContext context)
        {
            // Check if data already exists
            var existingTheatres = await context.Theatres.ToListAsync();
            if (existingTheatres.Any())
            {
                return; // Data already seeded
            }
            
            // Seed parent tables first
            var users = await SeedUsersAsync(context);
            var theatres = await SeedTheatresAsync(context);
            var movies = await SeedMoviesAsync(context);
            await context.SaveChangesAsync();
            
            // Then seed child tables
            await SeedMovieShowsAsync(context, movies, theatres);
            await context.SaveChangesAsync();
            
            await SeedTicketsAsync(context, users);
            await context.SaveChangesAsync();
        }

        private static async Task<List<Theatre>> SeedTheatresAsync(MiChitraDbContext context)
        {
            var theatres = new List<Theatre>
            {
                new Theatre { Name = "PVR Cinemas", City = "Mumbai" },
                new Theatre { Name = "INOX", City = "Delhi" },
                new Theatre { Name = "Cinepolis", City = "Bangalore" }
            };
            foreach (var theatre in theatres)
            {
                context.Theatres.Add(theatre);
            }
            return theatres;
        }

        private static async Task<List<User>> SeedUsersAsync(MiChitraDbContext context)
        {
            var users = new List<User>
            {
                new User {
                    FName= "Jane",
                    LName = "Doe",
                    Username = "admin1",
                    Email = "admin@michitra.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123@"),
                    Role = UserRole.Admin },
                new User {
                    FName= "John",
                    LName = "Smith",
                    Username = "user1",
                    Email = "john@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123@"),
                    Role = UserRole.User },
                new User {
                    FName= "Alice",
                    LName = "Johnson",
                    Username = "guest1",
                    Email = "alice@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Guest123@"),
                    Role = UserRole.Guest },
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            return users;
        }

        private static async Task<List<Movie>> SeedMoviesAsync(MiChitraDbContext context)
        {
            var movies = new List<Movie>
            {
                new Movie {
                    MovieName = "Inception",
                    Description = "A mind-bending thriller about dream invasion.",
                    Language = "English",
                    Rating = 8.8m
                },
                new Movie {
                    MovieName = "Barfi",
                    Description = "A romantic drama with a twist.",
                    Language = "Hindi",
                    Rating = 8.4m
                },
                new Movie {
                    MovieName = "Marley and Me",
                    Description = "A heartwarming story about a family and their dog.",
                    Language = "English",
                    Rating = 7.1m
                },
                new Movie {
                    MovieName = "3 Idiots",
                    Description = "A comedy-drama about friendship and life lessons.",
                    Language = "Hindi",
                    Rating = 8.4m
                },
                new Movie
                {
                    MovieName = "We Need To Talk About Kevin",
                    Description = "A psychological thriller exploring a mother's struggle with her son's dark nature.",
                    Language = "English",
                    Rating = 7.5m
                },
                new Movie
                {
                    MovieName = "Om Shanti Om",
                    Description = "A reincarnation-themed romantic drama set in Bollywood.",
                    Language = "Hindi",
                    Rating = 7.9m
                }
            };
            foreach (var movie in movies)
            {
                context.Movies.Add(movie);
            }
            return movies;
        }

        private static async Task SeedMovieShowsAsync(MiChitraDbContext context, List<Movie> movies, List<Theatre> theatres)
        {
            var movieShows = new List<MovieShow>
            {
                new MovieShow {
                    MovieId = movies[0].MovieId,
                    TheatreId = theatres[0].TheatreId,
                    ShowTime = DateTime.Now.AddDays(1).AddHours(10), 
                    TotalSeats = 100, 
                    AvailableSeats = 98, 
                    PricePerSeat = 250m },
                new MovieShow { 
                    MovieId = movies[0].MovieId,
                    TheatreId = theatres[1].TheatreId,
                    ShowTime = DateTime.Now.AddDays(1).AddHours(14), 
                    TotalSeats = 120, 
                    AvailableSeats = 119,
                    PricePerSeat = 300m },
                new MovieShow { 
                    MovieId = movies[1].MovieId,
                    TheatreId = theatres[0].TheatreId,
                    ShowTime = DateTime.Now.AddDays(2).AddHours(18),
                    TotalSeats = 100, 
                    AvailableSeats = 97, 
                    PricePerSeat = 200m },
                new MovieShow { 
                    MovieId = movies[2].MovieId,
                    TheatreId = theatres[2].TheatreId,
                    ShowTime = DateTime.Now.AddDays(1).AddHours(16), 
                    TotalSeats = 80,
                    AvailableSeats = 80,
                    PricePerSeat = 180m },
                new MovieShow { 
                    MovieId = movies[3].MovieId,
                    TheatreId = theatres[1].TheatreId,
                    ShowTime = DateTime.Now.AddDays(3).AddHours(20), 
                    TotalSeats = 120, 
                    AvailableSeats = 120,
                    PricePerSeat = 220m }
            };
            foreach (var show in movieShows)
            {
                context.MovieShows.Add(show);
            }
        }

        private static async Task SeedTicketsAsync(MiChitraDbContext context, List<User> users)
        {
            var movieShows = await context.MovieShows.ToListAsync();
            var showsList = movieShows.ToList();
            
            var tickets = new List<Ticket>
            {
                new Ticket { 
                    UserId = users[1].UserId,
                    MovieShowId = showsList[0].Id,
                    NumberOfSeats = 2, 
                    TotalPrice = 500m, 
                    BookingDate = DateTime.Now.AddDays(-1) },
                new Ticket { 
                    UserId = users[2].UserId,
                    MovieShowId = showsList[1].Id,
                    NumberOfSeats = 1, 
                    TotalPrice = 300m, 
                    BookingDate = DateTime.Now.AddHours(-5) },
                new Ticket { 
                    UserId = users[1].UserId,
                    MovieShowId = showsList[2].Id,
                    NumberOfSeats = 3, 
                    TotalPrice = 600m, 
                    BookingDate = DateTime.Now.AddHours(-2) }
            };
            foreach (var ticket in tickets)
            {
                context.Tickets.Add(ticket);
            }
        }
    }
}