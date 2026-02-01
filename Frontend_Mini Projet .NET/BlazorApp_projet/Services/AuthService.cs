using   BlazorApp1.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorApp1.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<UserInfo?> GetUserInfoAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IConfiguration _configuration;

        public AuthService(IApiService apiService, IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _apiService = apiService;
            _jsRuntime = jsRuntime;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var endpoint = _configuration["ApiSettings:AuthEndpoint"] + "/login";
                var response = await _apiService.PostAsync<LoginRequest, AuthResponse>(endpoint, request);

                if (response.IsAuthenticated)
                {
                    // Sauvegarder le token dans le localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem",
                        _configuration["JwtSettings:TokenKey"],
                        response.Token);

                    // Sauvegarder les infos utilisateur
                    var userInfo = new UserInfo
                    {
                        Username = response.Username,
                        Email = response.Email,
                        Roles = response.Roles,
                        Token = response.Token,
                        ExpiresOn = response.ExpiresOn
                    };

                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem",
                        _configuration["JwtSettings:UserKey"],
                        JsonSerializer.Serialize(userInfo));

                    // Configurer le token dans ApiService
                    _apiService.SetToken(response.Token);
                }

                return response;
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    IsAuthenticated = false,
                    Message = $"Erreur de connexion: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var endpoint = _configuration["ApiSettings:AuthEndpoint"] + "/register";
                return await _apiService.PostAsync<RegisterRequest, AuthResponse>(endpoint, request);
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    IsAuthenticated = false,
                    Message = $"Erreur d'inscription: {ex.Message}"
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _configuration["JwtSettings:TokenKey"]);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", _configuration["JwtSettings:UserKey"]);
            _apiService.SetToken(null);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem",
                _configuration["JwtSettings:TokenKey"]);
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem",
                _configuration["JwtSettings:UserKey"]);

            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonSerializer.Deserialize<UserInfo>(userJson);
            }

            return null;
        }
    }
}