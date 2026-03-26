using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Services;

namespace MiChitra.Tests;

[TestFixture]
public class MovieServiceTests
{
    private MiChitraDbContext _context = null!;
    private MovieService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<MiChitraDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new MiChitraDbContext(options);
        _service = new MovieService(_context, NullLogger<MovieService>.Instance);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    // --- GetAllMoviesAsync ---

    [Test]
    public async Task GetAllMoviesAsync_ReturnsAllMovies()
    {
        _context.Movies.AddRange(
            new Movie { MovieName = "Movie A", Language = "English", Rating = 8 },
            new Movie { MovieName = "Movie B", Language = "Tamil", Rating = 7 }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetAllMoviesAsync();

        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAllMoviesAsync_EmptyDb_ReturnsEmpty()
    {
        var result = await _service.GetAllMoviesAsync();
        Assert.That(result, Is.Empty);
    }

    // --- GetMovieByIdAsync ---

    [Test]
    public async Task GetMovieByIdAsync_ExistingId_ReturnsMovie()
    {
        var movie = new Movie { MovieName = "Test Movie", Language = "Hindi", Rating = 9 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var result = await _service.GetMovieByIdAsync(movie.MovieId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.MovieName, Is.EqualTo("Test Movie"));
    }

    [Test]
    public async Task GetMovieByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _service.GetMovieByIdAsync(999);
        Assert.That(result, Is.Null);
    }

    // --- CreateMovieAsync ---

    [Test]
    public async Task CreateMovieAsync_ValidDto_PersistsAndReturnsMovie()
    {
        var dto = new CreateMovieDto
        {
            MovieName = "New Movie",
            Description = "A great film",
            Language = "English",
            Rating = 8.5m
        };

        var result = await _service.CreateMovieAsync(dto);

        Assert.That(result.MovieId, Is.GreaterThan(0));
        Assert.That(result.MovieName, Is.EqualTo("New Movie"));
        Assert.That(_context.Movies.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateMovieAsync_SetsAvailabilityStatusToNoShows()
    {
        var dto = new CreateMovieDto { MovieName = "Solo", Language = "English", Rating = 7 };

        var result = await _service.CreateMovieAsync(dto);

        Assert.That(result.AvailabilityStatus, Is.EqualTo("No Shows"));
    }

    // --- UpdateMovieAsync ---

    [Test]
    public async Task UpdateMovieAsync_ExistingId_UpdatesAndReturnsTrue()
    {
        var movie = new Movie { MovieName = "Old Name", Language = "English", Rating = 6 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var dto = new UpdateMovieDto { MovieName = "New Name", Language = "Tamil", Rating = 8, Description = "Updated" };
        var result = await _service.UpdateMovieAsync(movie.MovieId, dto);

        Assert.That(result, Is.True);
        var updated = await _context.Movies.FindAsync(movie.MovieId);
        Assert.That(updated!.MovieName, Is.EqualTo("New Name"));
    }

    [Test]
    public async Task UpdateMovieAsync_NonExistingId_ReturnsFalse()
    {
        var dto = new UpdateMovieDto { MovieName = "X", Language = "English", Rating = 5 };
        var result = await _service.UpdateMovieAsync(999, dto);
        Assert.That(result, Is.False);
    }

    // --- DeleteMovieAsync ---

    [Test]
    public async Task DeleteMovieAsync_NoActiveShows_DeletesAndReturnsTrue()
    {
        var movie = new Movie { MovieName = "Delete Me", Language = "English", Rating = 5 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteMovieAsync(movie.MovieId);

        Assert.That(result, Is.True);
        Assert.That(_context.Movies.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteMovieAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _service.DeleteMovieAsync(999);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteMovieAsync_WithActiveShows_ThrowsInvalidOperationException()
    {
        var movie = new Movie { MovieName = "Active Movie", Language = "English", Rating = 7 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var theatre = new Theatre { Name = "PVR", City = "Chennai" };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        _context.MovieShows.Add(new MovieShow
        {
            MovieId = movie.MovieId,
            TheatreId = theatre.TheatreId,
            ShowTime = DateTime.UtcNow.AddDays(1),
            TotalSeats = 100,
            AvailableSeats = 50,
            PricePerSeat = 200
        });
        await _context.SaveChangesAsync();

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteMovieAsync(movie.MovieId));
    }

    // --- SearchMovieAsync ---

    [Test]
    public async Task SearchMovieAsync_MatchingName_ReturnsResults()
    {
        _context.Movies.AddRange(
            new Movie { MovieName = "Avengers", Language = "English", Rating = 9 },
            new Movie { MovieName = "Batman", Language = "English", Rating = 8 }
        );
        await _context.SaveChangesAsync();

        var result = await _service.SearchMovieAsync("Avengers");

        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().MovieName, Is.EqualTo("Avengers"));
    }

    [Test]
    public async Task SearchMovieAsync_NoMatch_ReturnsEmpty()
    {
        _context.Movies.Add(new Movie { MovieName = "Batman", Language = "English", Rating = 8 });
        await _context.SaveChangesAsync();

        var result = await _service.SearchMovieAsync("Superman");

        Assert.That(result, Is.Empty);
    }

    // --- AvailabilityStatus ---

    [Test]
    public async Task GetMovieByIdAsync_WithSoldOutShow_ReturnsStatusSoldOut()
    {
        var movie = new Movie { MovieName = "Sold Out Film", Language = "English", Rating = 8 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var theatre = new Theatre { Name = "INOX", City = "Mumbai" };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        _context.MovieShows.Add(new MovieShow
        {
            MovieId = movie.MovieId,
            TheatreId = theatre.TheatreId,
            ShowTime = DateTime.UtcNow.AddDays(1),
            TotalSeats = 100,
            AvailableSeats = 0,
            PricePerSeat = 150
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetMovieByIdAsync(movie.MovieId);

        Assert.That(result!.AvailabilityStatus, Is.EqualTo("Sold Out"));
    }

    [Test]
    public async Task GetMovieByIdAsync_WithAlmostFullShow_ReturnsStatusAlmostFull()
    {
        var movie = new Movie { MovieName = "Almost Full Film", Language = "English", Rating = 7 };
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        var theatre = new Theatre { Name = "Cinepolis", City = "Delhi" };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        _context.MovieShows.Add(new MovieShow
        {
            MovieId = movie.MovieId,
            TheatreId = theatre.TheatreId,
            ShowTime = DateTime.UtcNow.AddDays(1),
            TotalSeats = 100,
            AvailableSeats = 5,
            PricePerSeat = 150
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetMovieByIdAsync(movie.MovieId);

        Assert.That(result!.AvailabilityStatus, Is.EqualTo("Almost Full"));
    }
}
