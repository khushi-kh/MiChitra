using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.Models;

namespace MiChitra.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MiChitraDbContext context)
        {
            //Check if data already exists
            if (await context.Theatres.AnyAsync())
            {
                return; // Data already seeded
            }
            
            // Seed parent tables first
            var users = await SeedUsersAsync(context);
            var theatres = await SeedTheatresAsync(context);
            var movies = await SeedMoviesAsync(context);
            await context.SaveChangesAsync();
            
            // Then seed child tables using navigation properties
            await SeedMovieShowsAsync(context, movies, theatres);
            await context.SaveChangesAsync();
            
            await SeedTicketsAsync(context, users);
            await context.SaveChangesAsync();
        }

        // Seed Theatres
        private static async Task<List<Theatre>> SeedTheatresAsync(MiChitraDbContext context)
        {
            var theatres = new List<Theatre>
            {
                new Theatre { Name = "PVR Cinemas", City = "Mumbai" },
                new Theatre { Name = "INOX", City = "Delhi" },
                new Theatre { Name = "Cinepolis", City = "Bangalore" }
            };
            await context.Theatres.AddRangeAsync(theatres);
            return theatres;
        }

        // Seed Users
        private static async Task<List<User>> SeedUsersAsync(MiChitraDbContext context)
        {
            var users = new List<User>
            {
                //admin
                new User {
                    FName= "Jane",
                    LName = "Doe",
                    Username = "admin1",
                    Email = "admin@michitra.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123@"),
                    Role = UserRole.Admin },

                //User
                new User {
                    FName= "John",
                    LName = "Smith",
                    Username = "user1",
                    Email = "john@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123@"),
                    Role = UserRole.User },

                //Guest
                new User {
                    FName= "Alice",
                    LName = "Johnson",
                    Username = "guest1",
                    Email = "alice@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Guest123@"),
                    Role = UserRole.Guest },
            };
            await context.Users.AddRangeAsync(users);
            return users;
        }

        // Seed Movies
        private static async Task<List<Movie>> SeedMoviesAsync(MiChitraDbContext context)
        {
            var movies = new List<Movie>
            { new Movie {
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

            await context.Movies.AddRangeAsync(movies);
            return movies;
        }

        // Seed Movie Shows
        private static async Task SeedMovieShowsAsync(MiChitraDbContext context, List<Movie> movies, List<Theatre> theatres)
        {
            var movieShows = new List<MovieShow>
            {
                new MovieShow {
                    Movie = movies[0], 
                    Theatre = theatres[0], 
                    ShowTime = DateTime.Now.AddDays(1).AddHours(10), 
                    TotalSeats = 100, 
                    AvailableSeats = 98, 
                    PricePerSeat = 250m },

                new MovieShow { 
                    Movie = movies[0], 
                    Theatre = theatres[1], 
                    ShowTime = DateTime.Now.AddDays(1).AddHours(14), 
                    TotalSeats = 120, 
                    AvailableSeats = 119,
                    PricePerSeat = 300m },

                new MovieShow { 
                    Movie = movies[1], 
                    Theatre = theatres[0], 
                    ShowTime = DateTime.Now.AddDays(2).AddHours(18),
                    TotalSeats = 100, 
                    AvailableSeats = 97, 
                    PricePerSeat = 200m },

                new MovieShow { 
                    Movie = movies[2], 
                    Theatre = theatres[2], 
                    ShowTime = DateTime.Now.AddDays(1).AddHours(16), 
                    TotalSeats = 80,
                    AvailableSeats = 80,
                    PricePerSeat = 180m },

                new MovieShow { 
                    Movie = movies[3], 
                    Theatre = theatres[1], 
                    ShowTime = DateTime.Now.AddDays(3).AddHours(20), 
                    TotalSeats = 120, 
                    AvailableSeats = 120,
                    PricePerSeat = 220m }
            };
            await context.MovieShows.AddRangeAsync(movieShows);
        }

        private static async Task SeedTicketsAsync(MiChitraDbContext context, List<User> users)
        {
            var movieShows = await context.MovieShows.ToListAsync();
            
            var tickets = new List<Ticket>
            {
                new Ticket { 
                    User = users[1], 
                    MovieShow = movieShows[0], 
                    NumberOfSeats = 2, 
                    TotalPrice = 500m, 
                    BookingDate = DateTime.Now.AddDays(-1) },

                new Ticket { 
                    User = users[2], 
                    MovieShow = movieShows[1], 
                    NumberOfSeats = 1, 
                    TotalPrice = 300m, 
                    BookingDate = DateTime.Now.AddHours(-5) },

                new Ticket { 
                    User = users[1], 
                    MovieShow = movieShows[2],
                    NumberOfSeats = 3, 
                    TotalPrice = 600m, 
                    BookingDate = DateTime.Now.AddHours(-2) }
            };
            await context.Tickets.AddRangeAsync(tickets);
        }


    }
}
