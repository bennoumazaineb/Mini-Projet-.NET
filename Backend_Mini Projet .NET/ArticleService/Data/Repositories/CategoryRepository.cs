using Microsoft.EntityFrameworkCore;
using ArticleService.Models;

namespace ArticleService.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Articles)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Articles)
                .ToListAsync();
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.Now;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Articles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return false;

            // Vérifier si la catégorie contient des articles
            if (category.Articles.Any())
                throw new InvalidOperationException("Cannot delete category with existing articles");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetArticleCountAsync(int categoryId)
        {
            return await _context.Articles
                .CountAsync(a => a.CategoryId == categoryId);
        }
    }
}