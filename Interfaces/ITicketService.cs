using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync();
        Task<TicketResponseDTO?> GetTicketByIdAsync(int id);
        Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserAsync(int userId);
        Task<TicketResponseDTO> BookTicketAsync(BookTicketDTO dto);
        Task<bool> CancelTicketAsync(int ticketId);
    }
}