using System;
using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class UpdateMovieShowDTO
    {
        [Required]
        public DateTime ShowTime { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Total seats must be between 1 and 1000")]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Available seats must be between 0 and 1000")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0.01, 10000, ErrorMessage = "Price per seat must be between 0.01 and 10000")]
        public decimal PricePerSeat { get; set; }
    }
}