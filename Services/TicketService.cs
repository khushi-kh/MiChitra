using MiChitra.DTOs;
using MiChitra.Models;
using MiChitra.Interfaces;

namespace MiChitra.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                MovieShowId = t.MovieShowId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status
            });
        }

        public async Task<TicketResponseDTO?> GetTicketByIdAsync(int id)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
            if (ticket == null) return null;

            return new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                MovieShowId = ticket.MovieShowId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status
            };
        }

        public async Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserAsync(int userId)
        {
            var tickets = await _unitOfWork.Tickets.GetTicketsByUserIdAsync(userId);
            return tickets.Select(t => new TicketResponseDTO
            {
                TicketId = t.TicketId,
                UserId = t.UserId,
                MovieShowId = t.MovieShowId,
                BookingDate = t.BookingDate,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status
            });
        }

        public async Task<TicketResponseDTO> BookTicketAsync(BookTicketDTO dto)
        {
            var movieShow = await _unitOfWork.MovieShows.GetShowWithDetailsAsync(dto.MovieShowId);
            if (movieShow == null)
                throw new InvalidOperationException("Movie show not found");

            if (movieShow.AvailableSeats < dto.NumberOfSeats)
                throw new InvalidOperationException("Not enough seats available");

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

            return new TicketResponseDTO
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                MovieShowId = ticket.MovieShowId,
                BookingDate = ticket.BookingDate,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status
            };
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            if (ticket.Status == TicketStatus.Cancelled)
                throw new InvalidOperationException("Ticket is already cancelled");

            ticket.Status = TicketStatus.Cancelled;
            var movieShow = await _unitOfWork.MovieShows.GetByIdAsync(ticket.MovieShowId);
            if (movieShow != null)
            {
                movieShow.AvailableSeats += ticket.NumberOfSeats;
                _unitOfWork.MovieShows.Update(movieShow);
            }

            _unitOfWork.Tickets.Update(ticket);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}