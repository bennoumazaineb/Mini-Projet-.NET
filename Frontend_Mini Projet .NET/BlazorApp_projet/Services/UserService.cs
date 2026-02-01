using BlazorApp1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ILogger<UserService> _logger;

        // ✅ Base endpoint relatif (pas d'URL absolue)
        private readonly string _baseEndpoint = "gateway/auth/users";

        public UserService(
            HttpClient httpClient,
            IConfiguration configuration,
            IAuthService authService,
            ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Ajoute le token JWT dans le header Authorization
        /// </summary>
        private async Task AddAuthorizationHeader()
        {
            try
            {
                _logger.LogInformation("📥 Ajout du header d'autorisation...");
                var token = await _authService.GetTokenAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    _logger.LogInformation("✓ Token ajouté avec succès");
                }
                else
                {
                    _logger.LogWarning("⚠️ Token non trouvé ou vide");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de l'ajout du header d'autorisation");
            }
        }

        /// <summary>
        /// Récupère tous les utilisateurs
        /// </summary>
        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("📥 Début de GetAllUsersAsync");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE - Pas d'URL absolue
                _logger.LogInformation($"🔗 Appel API: GET {_baseEndpoint}");

                var response = await _httpClient.GetAsync(_baseEndpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {errorContent}");
                    return new List<UserResponse>();
                }

                var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                _logger.LogInformation($"✓ {users?.Count ?? 0} utilisateurs chargés");

                return users ?? new List<UserResponse>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans GetAllUsersAsync");
                return new List<UserResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur dans GetAllUsersAsync");
                return new List<UserResponse>();
            }
        }

        /// <summary>
        /// Récupère un utilisateur par ID
        /// </summary>
        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"📥 Début de GetUserByIdAsync pour l'ID: {id}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/{id}";
                _logger.LogInformation($"🔗 Appel API: GET {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"⚠️ Utilisateur non trouvé pour l'ID: {id}");
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {errorContent}");
                    return null;
                }

                var user = await response.Content.ReadFromJsonAsync<UserResponse>();
                _logger.LogInformation($"✓ Utilisateur chargé: {user?.Email}");

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Erreur dans GetUserByIdAsync pour l'ID: {id}");
                return null;
            }
        }

        /// <summary>
        /// Crée un nouvel utilisateur
        /// </summary>
        public async Task<string> CreateUserAsync(CreateUserModel model)
        {
            try
            {
                _logger.LogInformation($"📥 Début de CreateUserAsync pour: {model.Email}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                _logger.LogInformation($"🔗 Appel API: POST {_baseEndpoint}");

                var response = await _httpClient.PostAsJsonAsync(_baseEndpoint, model);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Utilisateur créé avec succès: {model.Email}");
                    return string.Empty;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {errorContent}");
                return errorContent.Length > 0 ? errorContent : $"Erreur HTTP {response.StatusCode}";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans CreateUserAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception dans CreateUserAsync");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        public async Task<string> UpdateUserAsync(string id, UpdateUserModel model)
        {
            try
            {
                _logger.LogInformation($"📥 Début de UpdateUserAsync pour l'ID: {id}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/update/{id}";
                _logger.LogInformation($"🔗 Appel API: PUT {endpoint}");

                var response = await _httpClient.PutAsJsonAsync(endpoint, model);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Mise à jour réussie pour l'ID: {id}");
                    return string.Empty;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {errorContent}");
                return errorContent.Length > 0 ? errorContent : $"Erreur HTTP {response.StatusCode}";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans UpdateUserAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Exception dans UpdateUserAsync pour l'ID: {id}");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        public async Task<string> DeleteUserAsync(string id)
        {
            try
            {
                _logger.LogInformation($"📥 Début de DeleteUserAsync pour l'ID: {id}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/delete/{id}";
                _logger.LogInformation($"🔗 Appel API: DELETE {endpoint}");

                var response = await _httpClient.DeleteAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Suppression réussie pour l'ID: {id}");
                    return string.Empty;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {errorContent}");
                return errorContent.Length > 0 ? errorContent : $"Erreur HTTP {response.StatusCode}";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans DeleteUserAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Exception dans DeleteUserAsync pour l'ID: {id}");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Ajoute un rôle à un utilisateur
        /// </summary>
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            try
            {
                _logger.LogInformation($"📥 Début de AddRoleAsync - UserId: {model.UserId}, Role: {model.Role}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/affect-role";
                _logger.LogInformation($"🔗 Appel API: POST {endpoint}");

                var response = await _httpClient.PostAsJsonAsync(endpoint, model);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Rôle {model.Role} ajouté à l'utilisateur {model.UserId}");
                    return string.Empty;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {errorContent}");
                return errorContent.Length > 0 ? errorContent : $"Erreur HTTP {response.StatusCode}";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans AddRoleAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception dans AddRoleAsync");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Récupère tous les clients
        /// </summary>
        public async Task<List<UserResponse>> GetClientsAsync()
        {
            try
            {
                _logger.LogInformation("📥 Début de GetClientsAsync");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/clients";
                _logger.LogInformation($"🔗 Appel API: GET {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {errorContent}");
                    return new List<UserResponse>();
                }

                var clients = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                _logger.LogInformation($"✓ {clients?.Count ?? 0} clients chargés");

                return clients ?? new List<UserResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur dans GetClientsAsync");
                return new List<UserResponse>();
            }
        }

        /// <summary>
        /// Récupère tous les responsables SAV
        /// </summary>
        public async Task<List<UserResponse>> GetResponsablesAsync()
        {
            try
            {
                _logger.LogInformation("📥 Début de GetResponsablesAsync");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/responsables";
                _logger.LogInformation($"🔗 Appel API: GET {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {errorContent}");
                    return new List<UserResponse>();
                }

                var responsables = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                _logger.LogInformation($"✓ {responsables?.Count ?? 0} responsables chargés");

                return responsables ?? new List<UserResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur dans GetResponsablesAsync");
                return new List<UserResponse>();
            }
        }

        /// <summary>
        /// Recherche les utilisateurs par email et/ou nom
        /// </summary>
        public async Task<List<UserResponse>> SearchUsersAsync(string email = "", string name = "")
        {
            try
            {
                _logger.LogInformation($"📥 Début de SearchUsersAsync - Email: {email}, Name: {name}");
                await AddAuthorizationHeader();

                // ✅ URL RELATIVE
                var endpoint = $"{_baseEndpoint}/search?email={Uri.EscapeDataString(email)}&name={Uri.EscapeDataString(name)}";
                _logger.LogInformation($"🔗 Appel API: GET {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {errorContent}");
                    return new List<UserResponse>();
                }

                var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                _logger.LogInformation($"✓ {users?.Count ?? 0} utilisateurs trouvés");

                return users ?? new List<UserResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur dans SearchUsersAsync");
                return new List<UserResponse>();
            }
        }
    }
}