using System.Text;
using System.Text.Json;
using BlazorApp1.Models;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CategoryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5241";
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var url = $"{_baseUrl}/gateway/categories";
            Console.WriteLine($"GET {url}");

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new();

            return JsonSerializer.Deserialize<List<CategoryResponse>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();
        }

        // =========================
        // CREATE
        // =========================
        public async Task<CategoryResponse?> CreateCategoryAsync(CategoryCreateDto category)
        {
            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/gateway/categories",
                new StringContent(
                    JsonSerializer.Serialize(category),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<CategoryResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/gateway/categories/{id}"
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<CategoryResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<CategoryResponse?> UpdateCategoryAsync(
            int id,
            CategoryUpdateDto category)
        {
            var response = await _httpClient.PutAsync(
                $"{_baseUrl}/gateway/categories/{id}",
                new StringContent(
                    JsonSerializer.Serialize(category),
                    Encoding.UTF8,
                    "application/json"
                )
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonSerializer.Deserialize<CategoryResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"{_baseUrl}/gateway/categories/{id}"
            );

            return response.IsSuccessStatusCode;
        }

        // =========================
        // SEARCH
        // =========================
        public async Task<List<CategoryResponse>> SearchCategoriesAsync(string term)
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/gateway/categories/search?term={Uri.EscapeDataString(term)}"
            );

            if (!response.IsSuccessStatusCode)
                return new();

            return JsonSerializer.Deserialize<List<CategoryResponse>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();
        }
    }
}
