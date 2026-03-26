using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Services;

namespace MiChitra.Tests;

[TestFixture]
public class TicketServiceTests
{
    private MiChitraDbContext _context = null!;
    private TicketService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<MiChitraDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new MiChitraDbContext(options);
        _service = new TicketService(_context, NullLogger<TicketService>.Instance);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private async Task<(Movie movie, Theatre theatre, MovieShow show, User user)> SeedBasicDataAsync(int availableSeats = 100)
    {
        var movie = new Movie { MovieName = "Test Movie", Language = "English", Rating = 8 };
        var theatre = new Theatre { Name = "PVR", City = "Mumbai" };
        _context.Movies.Add(movie);
        _context.Theatres.Add(theatre);
        await _context.SaveChangesAsync();

        var show = new MovieShow
        {
            MovieId = movie.MovieId,
            TheatreId = theatre.TheatreId,
            ShowTime = DateTime.UtcNow.AddDays(1),
            TotalSeats = 100,
            AvailableSeats = availableSeats,
            PricePerSeat = 200
        };
        _context.MovieShows.Add(show);

        var user = new User { FName = "John", LName = "Doe", Username = "johndoe", Email = "john@example.com", PasswordHash = "hash", Role = UserRole.User };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (movie, theatre, show, user);
    }

    // --- BookTicketAsync ---

    [Test]
    public async Task BookTicketAsync_ValidRequest_CreatesTicketAndReducesSeats()
    {
        var (_, _, show, user) = await SeedBasicDataAsync(100);

        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 3, SeatNumbers = ["A1", "A2", "A3"] };
        var result = await _service.BookTicketAsync(dto);

        Assert.That(result.TicketId, Is.GreaterThan(0));
        Assert.That(result.NumberOfSeats, Is.EqualTo(3));
        Assert.That(result.TotalPrice, Is.EqualTo(600));
        Assert.That(result.Status, Is.EqualTo(TicketStatus.Reserved));

        var updatedShow = await _context.MovieShows.FindAsync(show.Id);
        Assert.That(updatedShow!.AvailableSeats, Is.EqualTo(97));
    }

    [Test]
    public async Task BookTicketAsync_NotEnoughSeats_ThrowsInvalidOperationException()
    {
        var (_, _, show, user) = await SeedBasicDataAsync(2);

        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 5 };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.BookTicketAsync(dto));
    }

    [Test]
    public async Task BookTicketAsync_InvalidShowId_ThrowsInvalidOperationException()
    {
        var dto = new BookTicketDTO { UserId = 1, MovieShowId = 999, NumberOfSeats = 1 };
        Assert.ThrowsAsync<InvalidOperationException>(() => _service.BookTicketAsync(dto));
    }

    [Test]
    public async Task BookTicketAsync_ZeroSeats_ThrowsInvalidOperationException()
    {
        var (_, _, show, user) = await SeedBasicDataAsync();
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 0 };
        Assert.ThrowsAsync<InvalidOperationException>(() => _service.BookTicketAsync(dto));
    }

    // --- CancelTicketAsync ---

    [Test]
    public async Task CancelTicketAsync_ExistingTicket_CancelsAndRestoresSeats()
    {
        var (_, _, show, user) = await SeedBasicDataAsync(100);
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 2 };
        var booked = await _service.BookTicketAsync(dto);

        var result = await _service.CancelTicketAsync(booked.TicketId);

        Assert.That(result, Is.True);
        var ticket = await _context.Tickets.FindAsync(booked.TicketId);
        Assert.That(ticket!.Status, Is.EqualTo(TicketStatus.Cancelled));

        var updatedShow = await _context.MovieShows.FindAsync(show.Id);
        Assert.That(updatedShow!.AvailableSeats, Is.EqualTo(100));
    }

    [Test]
    public async Task CancelTicketAsync_NonExistingTicket_ReturnsFalse()
    {
        var result = await _service.CancelTicketAsync(999);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CancelTicketAsync_AlreadyCancelled_ReturnsTrue()
    {
        var (_, _, show, user) = await SeedBasicDataAsync();
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 1 };
        var booked = await _service.BookTicketAsync(dto);
        await _service.CancelTicketAsync(booked.TicketId);

        // Cancel again
        var result = await _service.CancelTicketAsync(booked.TicketId);
        Assert.That(result, Is.True);
    }

    // --- GetTicketsByUserIdAsync ---

    [Test]
    public async Task GetTicketsByUserIdAsync_ReturnsOnlyUserTickets()
    {
        var (_, _, show, user) = await SeedBasicDataAsync();
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 1 };
        await _service.BookTicketAsync(dto);

        var result = await _service.GetTicketsByUserIdAsync(user.UserId);

        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().UserId, Is.EqualTo(user.UserId));
    }

    [Test]
    public async Task GetTicketsByUserIdAsync_NoTickets_ReturnsEmpty()
    {
        var result = await _service.GetTicketsByUserIdAsync(999);
        Assert.That(result, Is.Empty);
    }

    // --- GetBookedSeatsAsync ---

    [Test]
    public async Task GetBookedSeatsAsync_ReturnsBookedSeats()
    {
        var (_, _, show, user) = await SeedBasicDataAsync();
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 2, SeatNumbers = ["B1", "B2"] };
        await _service.BookTicketAsync(dto);

        var seats = await _service.GetBookedSeatsAsync(show.Id);

        Assert.That(seats, Does.Contain("b1"));
        Assert.That(seats, Does.Contain("b2"));
    }

    [Test]
    public async Task GetBookedSeatsAsync_NoBookings_ReturnsEmpty()
    {
        var seats = await _service.GetBookedSeatsAsync(999);
        Assert.That(seats, Is.Empty);
    }

    // --- GetTicketByIdAsync ---

    [Test]
    public async Task GetTicketByIdAsync_ExistingId_ReturnsTicket()
    {
        var (_, _, show, user) = await SeedBasicDataAsync();
        var dto = new BookTicketDTO { UserId = user.UserId, MovieShowId = show.Id, NumberOfSeats = 1 };
        var booked = await _service.BookTicketAsync(dto);

        var result = await _service.GetTicketByIdAsync(booked.TicketId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TicketId, Is.EqualTo(booked.TicketId));
    }

    [Test]
    public async Task GetTicketByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _service.GetTicketByIdAsync(999);
        Assert.That(result, Is.Null);
    }
}
