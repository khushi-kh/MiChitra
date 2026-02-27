using MiChitra.DTOs;
using Microsoft.AspNetCore.Mvc;
using MiChitra.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MiChitra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // any authenticated user can pay/refund, ownership checked per ticket
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ITicketService _ticketService;

        public PaymentController(IPaymentService paymentService, ITicketService ticketService)
        {
            _paymentService = paymentService;
            _ticketService = ticketService;
        }

        [HttpPost("process")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("Admin");
                
                var ticket = await _ticketService.GetTicketByIdForUserAsync(dto.TicketId, userId, isAdmin);
                if (ticket == null)
                    return NotFound("Ticket not found");

                var paymentResult = await _paymentService.ProcessPaymentAsync(dto);
                return Ok(paymentResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(string paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return NotFound("Payment not found");
            return Ok(payment);
        }

        [HttpPost("refund/{ticketId}")]
        [EnableRateLimiting("BookingPolicy")]
        public async Task<IActionResult> ProcessRefund(int ticketId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("Admin");
                
                var ticket = await _ticketService.GetTicketByIdForUserAsync(ticketId, userId, isAdmin);
                if (ticket == null)
                    return NotFound("Ticket not found");

                var refundResult = await _paymentService.ProcessRefundAsync(ticketId);
                return Ok(refundResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}