using ArticleService.Models;

namespace ArticleService.Data.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> AddAsync(Category category);
        Task<Category> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<Category> GetByNameAsync(string name);
        Task<bool> ExistsAsync(int id);
        Task<int> GetArticleCountAsync(int categoryId);
    }
}