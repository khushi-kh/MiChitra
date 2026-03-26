using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Services;

namespace MiChitra.Tests;

[TestFixture]
public class TheatreServiceTests
{
    private MiChitraDbContext _context = null!;
    private TheatreService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<MiChitraDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new MiChitraDbContext(options);
        _service = new TheatreService(_context, NullLogger<TheatreService>.Instance);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    // --- GetAllTheatresAsync ---

    [Test]
    public async Task GetAllTheatresAsync_ReturnsAllTheatres()
    {
        _context.Theatres.AddRange(
            new Theatre { Name = "PVR", City = "Mumbai" },
            new Theatre { Name = "INOX", City = "Delhi" }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetAllTheatresAsync();

        Assert.That(result.Count(), Is.EqualTo(2));
    }

    // --- GetTheatreByIdAsync ---

    [Test]
    public async Task GetTheatreByIdAsync_ExistingId_ReturnsTheatre()
    {
        var theatre = new Theatre { Name = "Cinepolis", City = "Bangalore" };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var result = await _service.GetTheatreByIdAsync(theatre.TheatreId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Cinepolis"));
    }

    [Test]
    public async Task GetTheatreByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _service.GetTheatreByIdAsync(999);
        Assert.That(result, Is.Null);
    }

    // --- CreateTheatreAsync ---

    [Test]
    public async Task CreateTheatreAsync_ValidDto_PersistsAndReturnsTheatre()
    {
        var dto = new CreateTheatreDTO { Name = "New Theatre", City = "Hyderabad" };

        var result = await _service.CreateTheatreAsync(dto);

        Assert.That(result.TheatreId, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("New Theatre"));
        Assert.That(result.IsActive, Is.True);
    }

    // --- UpdateTheatreAsync ---

    [Test]
    public async Task UpdateTheatreAsync_ExistingId_UpdatesAndReturnsTrue()
    {
        var theatre = new Theatre { Name = "Old Name", City = "Chennai" };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var dto = new UpdateTheatreDTO { Name = "New Name", City = "Pune" };
        var result = await _service.UpdateTheatreAsync(theatre.TheatreId, dto);

        Assert.That(result, Is.True);
        var updated = await _context.Theatres.FindAsync(theatre.TheatreId);
        Assert.That(updated!.Name, Is.EqualTo("New Name"));
        Assert.That(updated.City, Is.EqualTo("Pune"));
    }

    [Test]
    public async Task UpdateTheatreAsync_NonExistingId_ReturnsFalse()
    {
        var dto = new UpdateTheatreDTO { Name = "X", City = "Y" };
        var result = await _service.UpdateTheatreAsync(999, dto);
        Assert.That(result, Is.False);
    }

    // --- DeactivateTheatreAsync ---

    [Test]
    public async Task DeactivateTheatreAsync_NoActiveShows_DeactivatesAndReturnsTrue()
    {
        var theatre = new Theatre { Name = "PVR", City = "Mumbai", isActive = true };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var result = await _service.DeactivateTheatreAsync(theatre.TheatreId);

        Assert.That(result, Is.True);
        var updated = await _context.Theatres.FindAsync(theatre.TheatreId);
        Assert.That(updated!.isActive, Is.False);
    }

    [Test]
    public async Task DeactivateTheatreAsync_WithActiveShows_ThrowsInvalidOperationException()
    {
        var theatre = new Theatre { Name = "INOX", City = "Delhi", isActive = true };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var movie = new Movie { MovieName = "Test", Language = "English", Rating = 7 };
        _context.Movies.Add(movie);
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

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeactivateTheatreAsync(theatre.TheatreId));
    }

    [Test]
    public async Task DeactivateTheatreAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _service.DeactivateTheatreAsync(999);
        Assert.That(result, Is.False);
    }

    // --- ActivateTheatreAsync ---

    [Test]
    public async Task ActivateTheatreAsync_InactiveTheatre_ActivatesAndReturnsTrue()
    {
        var theatre = new Theatre { Name = "PVR", City = "Mumbai", isActive = false };
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var result = await _service.ActivateTheatreAsync(theatre.TheatreId);

        Assert.That(result, Is.True);
        var updated = await _context.Theatres.FindAsync(theatre.TheatreId);
        Assert.That(updated!.isActive, Is.True);
    }

    // --- GetTheatresByCityAsync ---

    [Test]
    public async Task GetTheatresByCityAsync_ReturnsOnlyMatchingCity()
    {
        _context.Theatres.AddRange(
            new Theatre { Name = "PVR", City = "Mumbai" },
            new Theatre { Name = "INOX", City = "Delhi" }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetTheatresByCityAsync("Mumbai");

        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().City, Is.EqualTo("Mumbai"));
    }

    [Test]
    public async Task GetTheatresByCityAsync_CaseInsensitive_ReturnsResults()
    {
        _context.Theatres.Add(new Theatre { Name = "PVR", City = "Mumbai" });
        await _context.SaveChangesAsync();

        var result = await _service.GetTheatresByCityAsync("mumbai");

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    // --- GetActiveTheatresAsync ---

    [Test]
    public async Task GetActiveTheatresAsync_ReturnsOnlyActiveTheatres()
    {
        _context.Theatres.AddRange(
            new Theatre { Name = "Active", City = "Mumbai", isActive = true },
            new Theatre { Name = "Inactive", City = "Delhi", isActive = false }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetActiveTheatresAsync();

        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Name, Is.EqualTo("Active"));
    }

    // --- GetAllCitiesAsync ---

    [Test]
    public async Task GetAllCitiesAsync_ReturnsDistinctActiveCities()
    {
        _context.Theatres.AddRange(
            new Theatre { Name = "PVR", City = "Mumbai", isActive = true },
            new Theatre { Name = "INOX", City = "Mumbai", isActive = true },
            new Theatre { Name = "Cinepolis", City = "Delhi", isActive = true },
            new Theatre { Name = "Closed", City = "Pune", isActive = false }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetAllCitiesAsync();

        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result, Does.Contain("Mumbai"));
        Assert.That(result, Does.Contain("Delhi"));
    }
}
