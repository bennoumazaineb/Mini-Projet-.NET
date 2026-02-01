using ArticleService.Models;
using Microsoft.EntityFrameworkCore; // Ajouter cette ligne

namespace ArticleService.Data.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Article> AddAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<Article> GetByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<Article> UpdateAsync(Article article)
        {
            _context.Entry(article).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return false;
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Article> GetByReferenceAsync(string reference)
        {
            return await _context.Articles
                .FirstOrDefaultAsync(a => a.Reference == reference);
        }
    }
}
