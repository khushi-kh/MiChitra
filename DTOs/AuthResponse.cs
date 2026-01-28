namespace MiChitra.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        // Added: user payload
        public UserResponseDTO? User { get; set; }
    }

}