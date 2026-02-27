using MiChitra.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any authenticated user can view their own bookings
    public class BookingsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ITicketService ticketService, ILogger<BookingsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        /// <summary>
        /// Get all bookings (tickets) for the currently logged-in user.
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetCurrentUserBookings()
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
                return Unauthorized("Invalid user identifier in token.");

            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);
            return Ok(tickets);
        }
    }
}

