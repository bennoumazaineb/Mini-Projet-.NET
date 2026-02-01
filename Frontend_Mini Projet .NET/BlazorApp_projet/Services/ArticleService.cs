using System.Text;
using System.Text.Json;
using BlazorApp1.Models;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Services
{
    public interface IArticleService
    {
        Task<ArticleResponse?> CreateArticleAsync(ArticleCreateDto article);
        Task<ArticleResponse?> GetArticleByIdAsync(int id);
        Task<List<ArticleResponse>> GetAllArticlesAsync();
        Task<ArticleResponse?> UpdateArticleAsync(int id, ArticleUpdateDto article);
        Task<bool> DeleteArticleAsync(int id);
        Task<WarrantyCheckResponse> CheckWarrantyStatusAsync(int articleId, DateTime purchaseDate);
        Task<ArticleStats> GetArticleStatsAsync();
    }

    public class ArticleService : IArticleService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ArticleService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5241";
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<ArticleResponse>> GetAllArticlesAsync()
        {
            try
            {
                var url = $"{_baseUrl}/gateway/articles";
                Console.WriteLine($"GET {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<ArticleResponse>();

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<List<ArticleResponse>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetAllArticles: {ex.Message}");
                return new();
            }
        }

        // =========================
        // CREATE
        // =========================
        public async Task<ArticleResponse?> CreateArticleAsync(ArticleCreateDto article)
        {
            try
            {
                var url = $"{_baseUrl}/gateway/articles";

                var json = JsonSerializer.Serialize(article);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonSerializer.Deserialize<ArticleResponse>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur CreateArticle: {ex.Message}");
                return null;
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<ArticleResponse?> GetArticleByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/gateway/articles/{id}"
                );

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonSerializer.Deserialize<ArticleResponse>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetArticleById {id}: {ex.Message}");
                return null;
            }
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<ArticleResponse?> UpdateArticleAsync(int id, ArticleUpdateDto article)
        {
            try
            {
                var json = JsonSerializer.Serialize(article);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"{_baseUrl}/gateway/articles/{id}",
                    content
                );

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonSerializer.Deserialize<ArticleResponse>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur UpdateArticle {id}: {ex.Message}");
                return null;
            }
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteArticleAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{_baseUrl}/gateway/articles/{id}"
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur DeleteArticle {id}: {ex.Message}");
                return false;
            }
        }

        // =========================
        // WARRANTY
        // =========================
        public async Task<WarrantyCheckResponse> CheckWarrantyStatusAsync(
            int articleId,
            DateTime purchaseDate)
        {
            try
            {
                var url =
                    $"{_baseUrl}/gateway/articles/{articleId}/warranty-status" +
                    $"?purchaseDate={purchaseDate:yyyy-MM-dd}";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new WarrantyCheckResponse
                    {
                        ArticleId = articleId,
                        IsUnderWarranty = false
                    };

                return JsonSerializer.Deserialize<WarrantyCheckResponse>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                )!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Warranty {articleId}: {ex.Message}");
                return new WarrantyCheckResponse
                {
                    ArticleId = articleId,
                    IsUnderWarranty = false
                };
            }
        }

        // =========================
        // STATS (LOCAL)
        // =========================
        public async Task<ArticleStats> GetArticleStatsAsync()
        {
            var articles = await GetAllArticlesAsync();

            return new ArticleStats
            {
                TotalArticles = articles.Count,
                AvailableArticles = articles.Count(a => a.EstDisponible),
                OutOfStockArticles = articles.Count(a => !a.EstDisponible),
                TotalStockValue = articles.Sum(a => a.PrixAchat),
                AverageProfitMargin =
                    articles.Any() && articles.Sum(a => a.PrixAchat) > 0
                        ? articles.Average(a => ((a.PrixVente - a.PrixAchat) / a.PrixAchat) * 100)
                        : 0,
                CategoriesCount = articles
                    .Where(a => a.CategoryId.HasValue)
                    .Select(a => a.CategoryId!.Value)
                    .Distinct()
                    .Count(),
                UnderWarrantyCount = articles.Count(a => a.DureeGarantieMois > 0)
            };
        }
    }
}
