using ReclamationService.Models.DTOs;

namespace ReclamationService.Services
{
    public interface IAuthClientService
    {
        Task<bool> IsClientAsync(string userId);
        Task<bool> IsResponsableSAVAsync(string userId);
        Task<ClientInfoDTO> GetClientInfoAsync(string userId);
        Task<bool> ValidateUserExistsAsync(string userId);
    }

    public class ClientInfoDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}