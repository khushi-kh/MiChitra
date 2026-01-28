using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class BookTicketDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MovieShowId must be greater than 0")]
        public int MovieShowId { get; set; }
        
        [Required]
        [Range(1, 10, ErrorMessage = "Number of seats must be between 1 and 10")]
        public int NumberOfSeats { get; set; }
    }
}
