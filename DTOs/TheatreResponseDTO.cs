namespace MiChitra.DTOs
{
    public class TheatreResponseDTO
    {
        public int TheatreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}