using MiChitra.Data;
using MiChitra.Models;
using Microsoft.EntityFrameworkCore;

namespace MiChitra.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(MiChitraDbContext context)
        {
            if (await context.Theatres.AnyAsync())
                return;
            
            var users = SeedUsers(context);
            var theatres = SeedTheatres(context);
            var movies = SeedMovies(context);
            await context.SaveChangesAsync();
            
            SeedMovieShows(context, movies, theatres);
            await context.SaveChangesAsync();
            
            SeedTickets(context, users);
            await context.SaveChangesAsync();
        }

        private static List<Theatre> SeedTheatres(MiChitraDbContext context)
        {
            var theatres = new List<Theatre>
            {
                new Theatre { Name = "PVR Cinemas", City = "Mumbai", isActive = true },
                new Theatre { Name = "INOX", City = "Delhi", isActive = true },
                new Theatre { Name = "Cinepolis", City = "Bangalore", isActive = true }
            };
            context.Theatres.AddRange(theatres);
            return theatres;
        }

        private static List<User> SeedUsers(MiChitraDbContext context)
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
            context.Users.AddRange(users);
            return users;
        }

        private static List<Movie> SeedMovies(MiChitraDbContext context)
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
            context.Movies.AddRange(movies);
            return movies;
        }

        private static void SeedMovieShows(MiChitraDbContext context, List<Movie> movies, List<Theatre> theatres)
        {
            var movieShows = new List<MovieShow>
            {
                new MovieShow {
                    Movie = movies[0],
                    Theatre = theatres[0],
                    ShowTime = DateTime.UtcNow.AddDays(1).AddHours(10), 
                    TotalSeats = 100, 
                    AvailableSeats = 98, 
                    PricePerSeat = 250m,
                    Status = MovieShowStatus.Available },
                new MovieShow { 
                    Movie = movies[0],
                    Theatre = theatres[1],
                    ShowTime = DateTime.UtcNow.AddDays(1).AddHours(14), 
                    TotalSeats = 120, 
                    AvailableSeats = 119,
                    PricePerSeat = 300m,
                    Status = MovieShowStatus.Available },
                new MovieShow { 
                    Movie = movies[1],
                    Theatre = theatres[0],
                    ShowTime = DateTime.UtcNow.AddDays(2).AddHours(18),
                    TotalSeats = 100, 
                    AvailableSeats = 97, 
                    PricePerSeat = 200m,
                    Status = MovieShowStatus.Available },
                new MovieShow { 
                    Movie = movies[2],
                    Theatre = theatres[2],
                    ShowTime = DateTime.UtcNow.AddDays(1).AddHours(16), 
                    TotalSeats = 80,
                    AvailableSeats = 80,
                    PricePerSeat = 180m,
                    Status = MovieShowStatus.Available },
                new MovieShow { 
                    Movie = movies[3],
                    Theatre = theatres[1],
                    ShowTime = DateTime.UtcNow.AddDays(3).AddHours(20), 
                    TotalSeats = 120, 
                    AvailableSeats = 120,
                    PricePerSeat = 220m,
                    Status = MovieShowStatus.Available }
            };
            context.MovieShows.AddRange(movieShows);
        }

        private static void SeedTickets(MiChitraDbContext context, List<User> users)
        {
            var showsList = context.MovieShows.ToList();
            var tickets = new List<Ticket>
            {
                new Ticket { 
                    User = users[1],
                    MovieShow = showsList[0],
                    NumberOfSeats = 2, 
                    TotalPrice = 500m, 
                    BookingDate = DateTime.UtcNow.AddDays(-1) },
                new Ticket { 
                    User = users[2],
                    MovieShow = showsList[1],
                    NumberOfSeats = 1, 
                    TotalPrice = 300m, 
                    BookingDate = DateTime.UtcNow.AddHours(-5) },
                new Ticket { 
                    User = users[1],
                    MovieShow = showsList[2],
                    NumberOfSeats = 3, 
                    TotalPrice = 600m, 
                    BookingDate = DateTime.UtcNow.AddHours(-2) }
            };
            context.Tickets.AddRange(tickets);
        }
    }
}