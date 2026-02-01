using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ReclamationService.Services
{
    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthClientService> _logger;
        private readonly AuthServiceConfig _config;

        public AuthClientService(HttpClient httpClient, IOptions<AuthServiceConfig> config, ILogger<AuthClientService> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ReclamationService");
        }

        public async Task<bool> IsClientAsync(string userId)
        {
            try
            {
                var user = await GetClientInfoAsync(userId);
                return user?.Roles.Contains("Client") ?? false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsResponsableSAVAsync(string userId)
        {
            try
            {
                var user = await GetClientInfoAsync(userId);
                return user?.Roles.Contains("ResponsableSAV") ?? false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ClientInfoDTO> GetClientInfoAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/Auth/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientInfoDTO>();
                }

                _logger.LogWarning($"Utilisateur non trouvé: {userId}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur récupération client: {userId}");
                return null;
            }
        }

        public async Task<bool> ValidateUserExistsAsync(string userId)
        {
            var user = await GetClientInfoAsync(userId);
            return user != null;
        }
    }

    public class AuthServiceConfig
    {
        public string BaseUrl { get; set; } = "http://localhost:5000";
    }
}