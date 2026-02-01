using BlazorApp1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public class RoleService : IRoleService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ILogger<RoleService> _logger;

        // ✅ CORRECTION ICI : L'URL doit être "gateway/auth/api/roles"
        private const string _baseEndpoint = "gateway/auth/api/roles";

        public RoleService(
            HttpClient httpClient,
            IConfiguration configuration,
            IAuthService authService,
            ILogger<RoleService> logger)
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
                _logger.LogInformation("📥 Ajout du header d'autorisation pour RoleService...");
                var token = await _authService.GetTokenAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    // Nettoyer d'abord les anciens headers
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
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
        /// Récupère tous les rôles
        /// </summary>
        public async Task<List<RoleModel>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("📥 Début de GetAllRolesAsync");
                await AddAuthorizationHeader();

                _logger.LogInformation($"🔗 Appel API: GET {_baseEndpoint}");
                _logger.LogInformation($"🔗 BaseAddress HttpClient: {_httpClient.BaseAddress}");

                var response = await _httpClient.GetAsync(_baseEndpoint);
                _logger.LogInformation($"📊 Réponse Status: {response.StatusCode}");

                // Lire le contenu même en cas d'erreur pour le debug
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📊 Contenu réponse: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };

                        // Essayez différents formats de réponse
                        try
                        {
                            // Format 1: Tableau direct [{id, name}]
                            var roles = JsonSerializer.Deserialize<List<RoleModel>>(responseContent, options);
                            if (roles != null)
                            {
                                _logger.LogInformation($"✓ {roles.Count} rôles chargés (format tableau)");
                                return roles;
                            }
                        }
                        catch (JsonException)
                        {
                            // Format 2: Objet avec propriété "data" ou "roles"
                            try
                            {
                                var jsonDoc = JsonDocument.Parse(responseContent);
                                var root = jsonDoc.RootElement;

                                if (root.ValueKind == JsonValueKind.Object)
                                {
                                    if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                                    {
                                        var roles = JsonSerializer.Deserialize<List<RoleModel>>(dataElement.GetRawText(), options);
                                        _logger.LogInformation($"✓ {roles?.Count ?? 0} rôles chargés (via 'data')");
                                        return roles ?? new List<RoleModel>();
                                    }
                                    else if (root.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
                                    {
                                        var roles = JsonSerializer.Deserialize<List<RoleModel>>(rolesElement.GetRawText(), options);
                                        _logger.LogInformation($"✓ {roles?.Count ?? 0} rôles chargés (via 'roles')");
                                        return roles ?? new List<RoleModel>();
                                    }
                                    else if (root.ValueKind == JsonValueKind.Array)
                                    {
                                        var roles = JsonSerializer.Deserialize<List<RoleModel>>(responseContent, options);
                                        _logger.LogInformation($"✓ {roles?.Count ?? 0} rôles chargés (format tableau racine)");
                                        return roles ?? new List<RoleModel>();
                                    }
                                }
                            }
                            catch (Exception ex2)
                            {
                                _logger.LogError(ex2, "❌ Erreur de désérialisation alternative");
                            }
                        }

                        // Si on arrive ici, retourner une liste vide
                        return new List<RoleModel>();
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, $"❌ Erreur JSON: {responseContent}");
                        return new List<RoleModel>();
                    }
                }
                else
                {
                    _logger.LogError($"❌ Erreur HTTP {response.StatusCode}: {responseContent}");

                    // Pour debug, ajouter les rôles par défaut
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("⚠️ Endpoint non trouvé, retour des rôles par défaut");
                        return GetDefaultRoles();
                    }

                    return new List<RoleModel>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans GetAllRolesAsync");

                // Pour debug, retourner les rôles par défaut
                _logger.LogWarning("⚠️ Erreur réseau, retour des rôles par défaut");
                return GetDefaultRoles();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur dans GetAllRolesAsync");

                // Pour debug, retourner les rôles par défaut
                _logger.LogWarning("⚠️ Exception, retour des rôles par défaut");
                return GetDefaultRoles();
            }
        }

        /// <summary>
        /// Rôles par défaut pour le debug
        /// </summary>
        private List<RoleModel> GetDefaultRoles()
        {
            return new List<RoleModel>
            {
                new RoleModel { Id = "1", Name = "Admin", Description = "Administrateur système" },
                new RoleModel { Id = "2", Name = "ResponsableSAV", Description = "Responsable SAV" },
                new RoleModel { Id = "3", Name = "Client", Description = "Client" },
                new RoleModel { Id = "4", Name = "Manager", Description = "Manager personnalisé", IsSystemRole = false }
            };
        }

        /// <summary>
        /// Crée un nouveau rôle
        /// </summary>
        public async Task<string> CreateRoleAsync(string roleName)
        {
            try
            {
                _logger.LogInformation($"📥 Début de CreateRoleAsync pour: {roleName}");
                await AddAuthorizationHeader();

                _logger.LogInformation($"🔗 Appel API: POST {_baseEndpoint}");

                // IMPORTANT: Votre API attend juste une string, pas un objet JSON
                // Selon votre contrôleur: CreateRole([FromBody] string roleName)
                var content = new StringContent(
                    JsonSerializer.Serialize(roleName), // Juste la string
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_baseEndpoint, content);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📊 Contenu réponse: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Rôle créé avec succès: {roleName}");
                    return string.Empty; // Succès
                }
                else
                {
                    _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {responseContent}");

                    // Essayer d'extraire le message d'erreur
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        if (errorObj.TryGetProperty("error", out var errorProp) ||
                            errorObj.TryGetProperty("message", out errorProp) ||
                            errorObj.TryGetProperty("errors", out errorProp))
                        {
                            return errorProp.ToString();
                        }
                    }
                    catch { }

                    return responseContent.Length > 0 ? responseContent : $"Erreur HTTP {response.StatusCode}";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans CreateRoleAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception dans CreateRoleAsync");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Supprime un rôle
        /// </summary>
        public async Task<string> DeleteRoleAsync(string roleName)
        {
            try
            {
                _logger.LogInformation($"📥 Début de DeleteRoleAsync pour: {roleName}");
                await AddAuthorizationHeader();

                // URL encode le nom du rôle
                var encodedRoleName = Uri.EscapeDataString(roleName);
                var endpoint = $"{_baseEndpoint}/{encodedRoleName}";

                _logger.LogInformation($"🔗 Appel API: DELETE {endpoint}");

                var response = await _httpClient.DeleteAsync(endpoint);
                _logger.LogInformation($"📊 Réponse: {response.StatusCode}");

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📊 Contenu réponse: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✓ Rôle supprimé avec succès: {roleName}");
                    return string.Empty;
                }
                else
                {
                    _logger.LogWarning($"⚠️ Erreur {response.StatusCode}: {responseContent}");

                    // Essayer d'extraire le message d'erreur
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        if (errorObj.TryGetProperty("error", out var errorProp) ||
                            errorObj.TryGetProperty("message", out errorProp))
                        {
                            return errorProp.ToString();
                        }
                    }
                    catch { }

                    return responseContent.Length > 0 ? responseContent : $"Erreur HTTP {response.StatusCode}";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Erreur réseau dans DeleteRoleAsync");
                return $"Erreur réseau: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception dans DeleteRoleAsync");
                return $"Erreur: {ex.Message}";
            }
        }

        /// <summary>
        /// Récupère les utilisateurs d'un rôle spécifique
        /// </summary>
        public async Task<List<string>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                _logger.LogInformation($"📥 Début de GetUsersInRoleAsync pour le rôle: {roleName}");
                await AddAuthorizationHeader();

                // Note: Vous devrez peut-être créer cet endpoint dans votre API
                var encodedRoleName = Uri.EscapeDataString(roleName);
                var endpoint = $"gateway/auth/api/users/role/{encodedRoleName}";

                _logger.LogInformation($"🔗 Appel API: GET {endpoint}");

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<string>>();
                    _logger.LogInformation($"✓ {users?.Count ?? 0} utilisateurs trouvés pour le rôle {roleName}");
                    return users ?? new List<string>();
                }

                _logger.LogWarning($"⚠️ Endpoint non disponible {response.StatusCode} - Retour d'une liste vide");
                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Erreur dans GetUsersInRoleAsync pour le rôle: {roleName}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Test de connexion pour debug
        /// </summary>
        public async Task<string> TestConnectionAsync()
        {
            try
            {
                var testUrls = new[]
                {
                    "gateway/auth/api/roles",
                    "gateway/auth/roles",
                    "gateway/roles",
                    "api/roles"
                };

                var results = new List<string>();

                foreach (var url in testUrls)
                {
                    try
                    {
                        _logger.LogInformation($"🧪 Test URL: {url}");
                        var response = await _httpClient.GetAsync(url);
                        results.Add($"{url}: {response.StatusCode}");

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            results.Add($"  Erreur: {error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.Add($"{url}: ERROR - {ex.Message}");
                    }
                }

                return string.Join("\n", results);
            }
            catch (Exception ex)
            {
                return $"Test failed: {ex.Message}";
            }
        }
    }
}