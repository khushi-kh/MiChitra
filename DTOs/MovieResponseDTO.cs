namespace MiChitra.DTOs
{
    public class MovieResponseDto
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public decimal Rating { get; set; }
    }
}