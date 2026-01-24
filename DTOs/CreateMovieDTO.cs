namespace MiChitra.DTOs
{
    public class CreateMovieDto
    {
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public decimal Rating { get; set; }
    }
}
