using ArticleService.Models.DTOs;

namespace ArticleService.Services
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleResponseDto>> GetArticlesByCategoryAsync(int categoryId);
        Task<IEnumerable<ArticleResponseDto>> SearchArticlesAsync(string searchTerm);
        Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId);
        Task<bool> RemoveCategoryFromArticleAsync(int articleId);
        Task<ArticleResponseDto> CreateArticleAsync(ArticleCreateDto dto);
        Task<ArticleResponseDto> GetArticleByIdAsync(int id);
        Task<IEnumerable<ArticleResponseDto>> GetAllArticlesAsync();
        Task<ArticleResponseDto> UpdateArticleAsync(int id, ArticleCreateDto dto);
        Task<bool> DeleteArticleAsync(int id);
        Task<bool> IsArticleUnderWarrantyAsync(int articleId, DateTime purchaseDate);
    }
}
