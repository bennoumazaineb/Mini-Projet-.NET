using ArticleService.Data.Repositories;
using ArticleService.Models;
using ArticleService.Models.DTOs;
using AutoMapper;

namespace ArticleService.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ArticleResponseDto> CreateArticleAsync(ArticleCreateDto dto)
        {
            var article = _mapper.Map<Article>(dto);
            var created = await _repository.AddAsync(article);
            return _mapper.Map<ArticleResponseDto>(created);
        }

        public async Task<ArticleResponseDto> GetArticleByIdAsync(int id)
        {
            var article = await _repository.GetByIdAsync(id);
            return article == null ? null : _mapper.Map<ArticleResponseDto>(article);
        }

        public async Task<IEnumerable<ArticleResponseDto>> GetAllArticlesAsync()
        {
            var articles = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ArticleResponseDto>>(articles);
        }

        public async Task<ArticleResponseDto> UpdateArticleAsync(int id, ArticleCreateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return _mapper.Map<ArticleResponseDto>(updated);
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> IsArticleUnderWarrantyAsync(int articleId, DateTime purchaseDate)
        {
            var article = await _repository.GetByIdAsync(articleId);
            if (article == null) return false;

            var warrantyEndDate = purchaseDate.AddMonths(article.DureeGarantieMois);
            return DateTime.Now <= warrantyEndDate;
        }
        public async Task<IEnumerable<ArticleResponseDto>> GetArticlesByCategoryAsync(int categoryId)
        {
            var articles = await _repository.GetAllAsync();
            var filtered = articles.Where(a => a.CategoryId == categoryId);
            return _mapper.Map<IEnumerable<ArticleResponseDto>>(filtered);
        }

        public async Task<IEnumerable<ArticleResponseDto>> SearchArticlesAsync(string searchTerm)
        {
            var articles = await _repository.GetAllAsync();
            var filtered = articles.Where(a =>
                a.Designation.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.Reference.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return _mapper.Map<IEnumerable<ArticleResponseDto>>(filtered);
        }

        public async Task<bool> AssignCategoryToArticleAsync(int articleId, int categoryId)
        {
            var article = await _repository.GetByIdAsync(articleId);
            if (article == null) return false;

            article.CategoryId = categoryId;
            article.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> RemoveCategoryFromArticleAsync(int articleId)
        {
            var article = await _repository.GetByIdAsync(articleId);
            if (article == null) return false;

            article.CategoryId = null;
            article.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(article);
            return true;
        }
    }
}
