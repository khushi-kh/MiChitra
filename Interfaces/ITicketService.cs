using System.Collections.Generic;
using System.Threading.Tasks;
using MiChitra.DTOs;

namespace MiChitra.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketResponseDTO>> GetAllTicketsAsync();
        Task<TicketResponseDTO?> GetTicketByIdAsync(int id);
        Task<TicketResponseDTO?> GetTicketByIdForUserAsync(int ticketId, int userId, bool isAdmin);
        Task<TicketResponseDTO> BookTicketAsync(BookTicketDTO dto);
        Task<bool> CancelTicketAsync(int ticketId);
        Task<IEnumerable<TicketResponseDTO>> GetTicketsByUserIdAsync(int userId);
        Task<IEnumerable<TicketResponseDTO>> GetTicketsByShowIdAsync(int showId);
    }
}