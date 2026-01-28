using System;
using System.ComponentModel.DataAnnotations;

namespace MiChitra.DTOs
{
    public class CreateMovieShowDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MovieId must be greater than 0")]
        public int MovieId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TheatreId must be greater than 0")]
        public int TheatreId { get; set; }

        [Required]
        public DateTime ShowTime { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Total seats must be between 1 and 1000")]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0.01, 10000, ErrorMessage = "Price per seat must be between 0.01 and 10000")]
        public decimal PricePerSeat { get; set; }
    }
}