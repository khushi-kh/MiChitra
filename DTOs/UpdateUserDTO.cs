namespace MiChitra.DTOs
{
    // DTO for updating user profile
    public class UpdateUserDto
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
    }
}

