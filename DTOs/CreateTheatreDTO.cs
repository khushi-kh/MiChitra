using System.ComponentModel.DataAnnotations;
namespace MiChitra.DTOs
{
    public class CreateTheatreDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
    }
}
