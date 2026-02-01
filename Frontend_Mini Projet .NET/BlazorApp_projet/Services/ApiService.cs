// Services/ApiService.cs
using BlazorApp1.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorApp1.Services
{
    public interface IApiService
    {
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
        void SetToken(string? token);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private string? _token;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                // Créez la requête manuellement pour mieux contrôler
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

                // Ajoutez le token si disponible
                if (!string.IsNullOrEmpty(_token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }

                // Headers IMPORTANTS pour CORS
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                // Sérialisez le contenu
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envoyez la requête
                var response = await _httpClient.SendAsync(request);

                // Si c'est une redirection 307, réessayez avec la nouvelle URL
                if (response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect)
                {
                    var redirectUrl = response.Headers.Location?.ToString();
                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        // Recréez la requête pour l'URL de redirection
                        var redirectRequest = new HttpRequestMessage(HttpMethod.Post, redirectUrl)
                        {
                            Content = request.Content
                        };

                        // Copiez les headers
                        foreach (var header in request.Headers)
                        {
                            redirectRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }

                        // Ajoutez le header Origin pour CORS
                        redirectRequest.Headers.Add("Origin", "http://localhost:5241");

                        response = await _httpClient.SendAsync(redirectRequest);
                    }
                }

                // Lisez la réponse
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {responseContent}");
                }

                // Désérialisez la réponse
                return JsonSerializer.Deserialize<TResponse>(responseContent) ??
                    throw new InvalidOperationException("Réponse invalide");
            }
            catch (Exception ex)
            {
                // Pour le développement, créez une réponse mock
                return CreateMockResponse<TResponse>(endpoint, ex);
            }
        }

        public void SetToken(string? token)
        {
            _token = token;
        }

        private TResponse CreateMockResponse<TResponse>(string endpoint, Exception ex)
        {
            // SIMULATION POUR LE DÉVELOPPEMENT
            if (endpoint.Contains("login"))
            {
                // Créez une réponse mock pour le login
                var mockResponse = new AuthResponse
                {
                    IsAuthenticated = true,
                    Token = "dev-token-" + Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@test.com",
                    Roles = new List<string> { "Admin" },
                    Message = "Mode développement - Connexion simulée",
                    ExpiresOn = DateTime.Now.AddHours(1)
                };

                return (TResponse)(object)mockResponse;
            }

            if (endpoint.Contains("register"))
            {
                var mockResponse = new AuthResponse
                {
                    IsAuthenticated = true,
                    Token = "dev-token-" + Guid.NewGuid(),
                    Username = "user",
                    Email = "user@test.com",
                    Roles = new List<string> { "User" },
                    Message = "Mode développement - Inscription simulée",
                    ExpiresOn = DateTime.Now.AddHours(1)
                };

                return (TResponse)(object)mockResponse;
            }

            // Pour les autres types, retournez default
            return default!;
        }
    }
}