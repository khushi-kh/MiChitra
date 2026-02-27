using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class TicketService : ITicketService
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(MiChitraDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Theatre)
                .Include(t => t.User)
                .ToListAsync();
            return tickets.Select(MapToResponseDto);
        }

        public async Task<TicketResponseDTO?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Theatre)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TicketId == id);
            return ticket == null ? null : MapToResponseDto(ticket);
        }

        // New: returns ticket only if user owns it or caller is admin
        public async Task<TicketResponseDTO?> GetTicketByIdForUserAsync(int ticketId, int userId, bool isAdmin)
        {
            var ticket = await _context.Tickets
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Theatre)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null) return null;

            if (isAdmin) return MapToResponseDto(ticket);

            return ticket.UserId == userId ? MapToResponseDto(ticket) : null;
        }

        public async Task<TicketResponseDTO> BookTicketAsync(BookTicketDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get movie show with lock to prevent race conditions
                var movieShow = await _context.MovieShows
                    .FirstOrDefaultAsync(ms => ms.Id == dto.MovieShowId);

                if (movieShow == null)
                    throw new InvalidOperationException("Movie show not found");

                if (movieShow.AvailableSeats < dto.NumberOfSeats)
                    throw new InvalidOperationException("Not enough seats available");

                if (movieShow.ShowTime <= DateTime.UtcNow)
                    throw new InvalidOperationException("Cannot book tickets for past shows");

                // Validate requested seats (if provided) and ensure they are not already reserved/booked
                var requestedSeats = (dto.SeatNumbers ?? new List<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .ToList();

                if (requestedSeats.Count > 0)
                {
                    var distinctRequested = requestedSeats.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    if (distinctRequested.Count != requestedSeats.Count)
                        throw new InvalidOperationException("Duplicate seat numbers selected");

                    if (distinctRequested.Count != dto.NumberOfSeats)
                        throw new InvalidOperationException("Number of selected seats does not match NumberOfSeats");

                    var existingSeatStrings = await _context.Tickets
                        .Where(t =>
                            t.MovieShowId == dto.MovieShowId &&
                            t.Status != TicketStatus.Cancelled &&
                            t.Status != TicketStatus.Expired &&
                            t.SeatNumbers != null)
                        .Select(t => t.SeatNumbers!)
                        .ToListAsync();

                    var blockedSeats = existingSeatStrings
                        .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var conflicting = distinctRequested.Where(s => blockedSeats.Contains(s)).ToList();
                    if (conflicting.Count > 0)
                        throw new InvalidOperationException($"Selected seats are no longer available: {string.Join(", ", conflicting)}");
                }

                // Calculate total price
                var totalPrice = dto.NumberOfSeats * movieShow.PricePerSeat;

                // Update available seats
                movieShow.AvailableSeats -= dto.NumberOfSeats;

                // Update show status based on availability
                var occupancyPercentage = (double)(movieShow.TotalSeats - movieShow.AvailableSeats) / movieShow.TotalSeats;
                movieShow.Status = occupancyPercentage switch
                {
                    >= 1.0 => MovieShowStatus.SoldOut,
                    >= 0.8 => MovieShowStatus.AlmostFull,
                    _ => MovieShowStatus.Available
                };

                // Create ticket
                var ticket = new Ticket
                {
                    UserId = dto.UserId,
                    MovieShowId = dto.MovieShowId,
                    NumberOfSeats = dto.NumberOfSeats,
                    SeatNumbers = requestedSeats.Count > 0 ? string.Join(",", requestedSeats) : null,
                    TotalPrice = totalPrice,
                    BookingDate = DateTime.UtcNow,
                    Status = TicketStatus.Reserved,
                    ReservationExpiry = DateTime.UtcNow.AddMinutes(10),
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Ticket booked successfully. TicketId: {TicketId}, UserId: {UserId}",
                    ticket.TicketId, dto.UserId);

                // Reload ticket with includes for response
                var bookedTicket = await _context.Tickets
                    .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Movie)
                    .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Theatre)
                    .Include(t => t.User)
                    .FirstAsync(t => t.TicketId == ticket.TicketId);

                return MapToResponseDto(bookedTicket);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.MovieShow)
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null) return false;

                if (ticket.Status == TicketStatus.Cancelled)
                    throw new InvalidOperationException("Ticket is already cancelled");

                if (ticket.MovieShow.ShowTime <= DateTime.UtcNow.AddHours(2))
                    throw new InvalidOperationException("Cannot cancel tickets within 2 hours of show time");

                // Update ticket status
                ticket.Status = TicketStatus.Cancelled;

                // Return seats to available pool
                ticket.MovieShow.AvailableSeats += ticket.NumberOfSeats;

                // Update show status
                var occupancyPercentage = (double)(ticket.MovieShow.TotalSeats - ticket.MovieShow.AvailableSeats) / ticket.MovieShow.TotalSeats;
                ticket.MovieShow.Status = occupancyPercentage switch
                {
                    >= 1.0 => MovieShowStatus.SoldOut,
                    >= 0.8 => MovieShowStatus.AlmostFull,
                    _ => MovieShowStatus.Available
                };

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Ticket cancelled successfully. TicketId: {TicketId}", ticketId);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserIdAsync(int userId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.MovieShow)
                .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow.Theatre)
                .Include(t => t.Payment)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.BookingDate)
                .ToListAsync();
            return tickets.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByShowIdAsync(int showId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.User)
                .Where(t => t.MovieShowId == showId)
                .ToListAsync();
            return tickets.Select(MapToResponseDto);
        }

        public async Task<List<string>> GetBookedSeatsAsync(int movieShowId)
        {
            var tickets = await _context.Tickets
                .Where(t =>
                    t.MovieShowId == movieShowId &&
                    t.Status != TicketStatus.Cancelled &&
                    t.Status != TicketStatus.Expired)
                .ToListAsync();

            var bookedSeats = new List<string>();
            foreach (var ticket in tickets)
            {
                if (!string.IsNullOrEmpty(ticket.SeatNumbers))
                {
                    bookedSeats.AddRange(ticket.SeatNumbers.Split(','));
                }
            }
            return bookedSeats;
        }

        private static TicketResponseDTO MapToResponseDto(Ticket ticket)
        {
            return new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                MovieShowId = ticket.MovieShowId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status,
                ShowTime = ticket.MovieShow?.ShowTime ?? default,
                PricePerSeat = ticket.MovieShow?.PricePerSeat ?? 0,
                MovieName = ticket.MovieShow?.Movie?.MovieName,
                TheatreName = ticket.MovieShow?.Theatre?.Name,
                TransactionId = ticket.Payment?.TransactionId
            };
        }
    }
}