using MiChitra.Data;
using MiChitra.Models;
using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly MiChitraDbContext _context;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(MiChitraDbContext context, ILogger<TicketsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Theatre)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketResponseDTO
            {
                UserId = t.UserId,
                TicketId = t.TicketId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status,

                MovieShowId = t.MovieShow!.Id,
                ShowTime = t.MovieShow.ShowTime,
                PricePerSeat = t.MovieShow.PricePerSeat,

                MovieName = t.MovieShow.Movie!.MovieName,
                TheatreName = t.MovieShow.Theatre!.Name
            }).ToList();

            return Ok(ticketDtos);
        }


        // GET: api/tickets/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            _logger.LogInformation("Fetching ticket with ID {TicketId}", id);

            var ticket = await _context.Tickets
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Theatre)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket with ID {TicketId} not found", id);
                return NotFound("Ticket not found");
            }

            var dto = new TicketResponseDTO
            {
                UserId = ticket.UserId,
                TicketId = ticket.TicketId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status,

                MovieShowId = ticket.MovieShow!.Id,
                ShowTime = ticket.MovieShow.ShowTime,
                PricePerSeat = ticket.MovieShow.PricePerSeat,

                MovieName = ticket.MovieShow.Movie!.MovieName,
                TheatreName = ticket.MovieShow.Theatre!.Name
            };

            return Ok(dto);

        }

        // GET: api/tickets/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUser(int userId)
        {
            _logger.LogInformation("Fetching tickets for user {UserId}", userId);

            var tickets = await _context.Tickets
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Movie)
                .Include(t => t.MovieShow)
                    .ThenInclude(ms => ms.Theatre)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status,

                MovieShowId = t.MovieShow!.Id,
                ShowTime = t.MovieShow.ShowTime,
                PricePerSeat = t.MovieShow.PricePerSeat,

                MovieName = t.MovieShow.Movie!.MovieName,
                TheatreName = t.MovieShow.Theatre!.Name
            }).ToList();

            return Ok(ticketDtos);

        }

        // POST: api/tickets/book
        [HttpPost("book")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO dto)
        {
            _logger.LogInformation("Booking ticket for User {UserId}, Show {ShowId}, Seats {Seats}",
                dto.UserId, dto.MovieShowId, dto.NumberOfSeats);

            var movieShow = await _context.MovieShows
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theatre)
                .FirstOrDefaultAsync(ms => ms.Id == dto.MovieShowId);

            if (movieShow == null)
            {
                _logger.LogWarning("MovieShow {ShowId} not found", dto.MovieShowId);
                return NotFound("Movie show not found");
            }

            if (movieShow.AvailableSeats < dto.NumberOfSeats)
            {
                _logger.LogWarning("Not enough seats. Requested: {Requested}, Available: {Available}",
                    dto.NumberOfSeats, movieShow.AvailableSeats);
                return BadRequest("Not enough seats available");
            }

            var totalPrice = dto.NumberOfSeats * movieShow.PricePerSeat;

            movieShow.AvailableSeats -= dto.NumberOfSeats;

            var ticket = new Ticket
            {
                UserId = dto.UserId,
                MovieShowId = dto.MovieShowId,
                NumberOfSeats = dto.NumberOfSeats,
                TotalPrice = totalPrice,
                BookingDate = DateTime.UtcNow,
                Status = TicketStatus.Booked
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ticket booked successfully. TicketId: {TicketId}", ticket.TicketId);

            // Return DTO, not entity
            var response = new TicketResponseDTO
            {
                UserId = ticket.UserId,
                TicketId = ticket.TicketId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status,

                MovieShowId = movieShow.Id,
                ShowTime = movieShow.ShowTime,
                PricePerSeat = movieShow.PricePerSeat,

                MovieName = movieShow.Movie!.MovieName,
                TheatreName = movieShow.Theatre!.Name
            };

            return Ok(response);
        }


        // PUT: api/tickets/cancel/{ticketId}
        [HttpPut("cancel/{ticketId}")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            _logger.LogInformation("Cancelling ticket {TicketId}", ticketId);

            var ticket = await _context.Tickets
                .Include(t => t.MovieShow)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found", ticketId);
                return NotFound("Ticket not found");
            }

            if (ticket.Status == TicketStatus.Cancelled)
            {
                _logger.LogWarning("Ticket {TicketId} already cancelled", ticketId);
                return BadRequest("Ticket is already cancelled");
            }

            // Mark as cancelled
            ticket.Status = TicketStatus.Cancelled;

            // Release seats back
            ticket.MovieShow!.AvailableSeats += ticket.NumberOfSeats;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ticket {TicketId} cancelled successfully", ticketId);

            return Ok("Ticket cancelled successfully");
        }
    }
}
