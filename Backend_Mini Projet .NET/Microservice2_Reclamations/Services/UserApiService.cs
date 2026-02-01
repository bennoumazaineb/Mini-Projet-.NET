using System.Text;
using System.Text.Json;
using Microservice2_Reclamations.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Microservice2_Reclamations.Services
{
    public interface IUserApiService
    {
        Task<UserDto?> GetUserByIdAsync(string userId, string authToken);
        Task<ValidateTokenResponseDto?> ValidateTokenAsync(string token);
    }

    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<UserApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Configurer l'URL de base du Microservice 1
            var authServiceUrl = _configuration["Microservices:AuthService:BaseUrl"]
                ?? "http://localhost:5184";
            _httpClient.BaseAddress = new Uri(authServiceUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId, string authToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{userId}");
                request.Headers.Add("Authorization", $"Bearer {authToken}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDto>(content, _jsonOptions);

                    _logger.LogInformation("Utilisateur {UserId} récupéré avec succès", userId);
                    return user;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Utilisateur {UserId} non trouvé", userId);
                    return null;
                }
                else
                {
                    _logger.LogError("Erreur HTTP {StatusCode} pour l'utilisateur {UserId}",
                        response.StatusCode, userId);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur de connexion au Microservice 1 pour l'utilisateur {UserId}", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur inattendue pour l'utilisateur {UserId}", userId);
                return null;
            }
        }

        public async Task<ValidateTokenResponseDto?> ValidateTokenAsync(string token)
        {
            try
            {
                var validateDto = new ValidateTokenDto { Token = token };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(validateDto, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/api/auth/validate-token", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ValidateTokenResponseDto>(content, _jsonOptions);
                }

                _logger.LogWarning("Token invalide ou erreur de validation");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur validation token");
                return null;
            }
        }
    }
}