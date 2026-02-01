using ArticleService.Models;

namespace ArticleService.Data.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> AddAsync(Article article);
        Task<Article> GetByIdAsync(int id);
        Task<IEnumerable<Article>> GetAllAsync();
        Task<Article> UpdateAsync(Article article);
        Task<bool> DeleteAsync(int id);
        Task<Article> GetByReferenceAsync(string reference);
    }
}
