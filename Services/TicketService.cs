using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.DTOs;
using MiChitra.Interfaces;
using MiChitra.Models;
using System.Text.Json;

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
                City = ticket.MovieShow?.Theatre?.City,
                TransactionId = ticket.Payment?.TransactionId,
                ReservationExpiry = ticket.ReservationExpiry,
                SeatNumbers = ParseSeatStringToList(ticket.SeatNumbers) ?? new List<string>()
            };
        }

        private static List<string>? ParseSeatStringToList(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var trimmed = raw.Trim();
            if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) ||
                (trimmed.StartsWith("\"[") && trimmed.EndsWith("]\"")))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<List<string>>(trimmed);
                    if (parsed != null) return parsed.Select(s => s?.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                }
                catch
                {
                }
            }

            return trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                          .Select(s => s.Trim())
                          .Where(s => !string.IsNullOrEmpty(s))
                          .ToList();
        }

        private static string? ConvertSeatListToString(List<string>? seats)
        {
            if (seats == null || seats.Count == 0) return null;
            return string.Join(',', seats);
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Theatre)
                .Include(t => t.Payment)
                .ToListAsync();

            return tickets.Select(MapToResponseDto);
        }

        public async Task<TicketResponseDTO?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Theatre)
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            return ticket == null ? null : MapToResponseDto(ticket);
        }

        public async Task<TicketResponseDTO?> GetTicketByIdForUserAsync(int ticketId, int userId, bool isAdmin)
        {
            var ticket = await _context.Tickets
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Theatre)
                .Include(t => t.Payment)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null) return null;
            if (!isAdmin && ticket.UserId != userId) return null;
            return MapToResponseDto(ticket);
        }

        public async Task<TicketResponseDTO> BookTicketAsync(DTOs.BookTicketDTO dto)
        {
            var show = await _context.MovieShows.FirstOrDefaultAsync(ms => ms.Id == dto.MovieShowId);
            if (show == null) throw new InvalidOperationException("Show not found");

            if (dto.NumberOfSeats <= 0) throw new InvalidOperationException("Invalid seat count");

            if (show.AvailableSeats < dto.NumberOfSeats) throw new InvalidOperationException("Not enough available seats");

            var ticket = new Ticket
            {
                UserId = dto.UserId,
                MovieShowId = dto.MovieShowId,
                NumberOfSeats = dto.NumberOfSeats,
                TotalPrice = dto.NumberOfSeats * show.PricePerSeat,
                BookingDate = DateTime.UtcNow,
                Status = TicketStatus.Reserved,
                ReservationExpiry = DateTime.UtcNow.AddMinutes(10), // default reservation window
                SeatNumbers = ConvertSeatListToString(dto.SeatNumbers)
            };

            _logger.LogInformation("BookTicketAsync: persisting SeatNumbers string for Ticket (UserId={UserId}, ShowId={ShowId}): {SeatNumbersString}",
                dto.UserId, dto.MovieShowId, ticket.SeatNumbers);

            _context.Tickets.Add(ticket);

            show.AvailableSeats -= dto.NumberOfSeats;

            await _context.SaveChangesAsync();

            await _context.Entry(ticket).Reference(t => t.MovieShow).LoadAsync();
            await _context.Entry(ticket.MovieShow!).Reference(ms => ms.Movie).LoadAsync();
            await _context.Entry(ticket.MovieShow!).Reference(ms => ms.Theatre).LoadAsync();

            return MapToResponseDto(ticket);
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;

            if (ticket.Status == TicketStatus.Cancelled) return true;

            ticket.Status = TicketStatus.Cancelled;
            ticket.UpdatedAt = DateTime.UtcNow;

            var show = await _context.MovieShows.FindAsync(ticket.MovieShowId);
            if (show != null)
            {
                show.AvailableSeats += ticket.NumberOfSeats;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserIdAsync(int userId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Theatre)
                .Include(t => t.Payment)
                .OrderByDescending(t => t.BookingDate)
                .ToListAsync();

            return tickets.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByShowIdAsync(int showId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.MovieShowId == showId)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow).ThenInclude(ms => ms.Theatre)
                .Include(t => t.Payment)
                .ToListAsync();

            return tickets.Select(MapToResponseDto);
        }

        public async Task<List<string>> GetBookedSeatsAsync(int movieShowId)
        {
            var tickets = await _context.Tickets
                .Where(t => t.MovieShowId == movieShowId && (t.Status == TicketStatus.Booked || t.Status == TicketStatus.Reserved))
                .ToListAsync();

            var seats = new List<string>();
            foreach (var t in tickets)
            {
                var parsed = ParseSeatStringToList(t.SeatNumbers);
                if (parsed != null)
                {
                    seats.AddRange(parsed);
                }
            }

            return seats.Select(s => s.Trim().ToLowerInvariant()).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        }
    }
}