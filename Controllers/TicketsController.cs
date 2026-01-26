using MiChitra.Models;
using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(IUnitOfWork unitOfWork, ILogger<TicketsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            var ticketDtos = tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                MovieShowId = t.MovieShowId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status
            });
            return Ok(ticketDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
            if (ticket == null)
                return NotFound("Ticket not found");
            
            var ticketDto = new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                MovieShowId = ticket.MovieShowId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status
            };
            return Ok(ticketDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUser(int userId)
        {
            var tickets = await _unitOfWork.Tickets.GetTicketsByUserIdAsync(userId);
            var ticketDtos = tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                MovieShowId = t.MovieShowId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status
            });
            return Ok(ticketDtos);
        }

        [HttpPost("book")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO dto)
        {
            var movieShow = await _unitOfWork.MovieShows.GetShowWithDetailsAsync(dto.MovieShowId);
            if (movieShow == null)
                return NotFound("Movie show not found");

            if (movieShow.AvailableSeats < dto.NumberOfSeats)
                return BadRequest("Not enough seats available");

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

            await _unitOfWork.Tickets.AddAsync(ticket);
            _unitOfWork.MovieShows.Update(movieShow);
            await _unitOfWork.SaveChangesAsync();

            var ticketDto = new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                MovieShowId = ticket.MovieShowId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status
            };
            return Ok(ticketDto);
        }

        [HttpPut("cancel/{ticketId}")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found");

            if (ticket.Status == TicketStatus.Cancelled)
                return BadRequest("Ticket is already cancelled");

            ticket.Status = TicketStatus.Cancelled;
            var movieShow = await _unitOfWork.MovieShows.GetByIdAsync(ticket.MovieShowId);
            if (movieShow != null)
            {
                movieShow.AvailableSeats += ticket.NumberOfSeats;
                _unitOfWork.MovieShows.Update(movieShow);
            }

            _unitOfWork.Tickets.Update(ticket);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Ticket cancelled successfully");
        }
    }
}
