namespace Microservice2_Reclamations.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
    }

    public class ValidateTokenDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenResponseDto
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}