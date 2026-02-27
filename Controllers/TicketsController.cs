using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any authenticated user can book/cancel tickets
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound("Ticket not found");
            return Ok(ticket);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUser(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("booked-seats/{movieShowId}")]
        public async Task<IActionResult> GetBookedSeats(int movieShowId)
        {
            var bookedSeats = await _ticketService.GetBookedSeatsAsync(movieShowId);
            return Ok(bookedSeats);
        }

        [HttpPost("book")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ticket = await _ticketService.BookTicketAsync(dto);
                return Ok(ticket);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cancel/{ticketId}")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            try
            {
                var success = await _ticketService.CancelTicketAsync(ticketId);
                if (!success)
                    return NotFound("Ticket not found");
                return Ok("Ticket cancelled successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}