using AutoMapper;
using ArticleService.Data.Repositories;
using ArticleService.Models;
using ArticleService.Models.DTOs;

namespace ArticleService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            // Vérifier si le nom existe déjà
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new ArgumentException($"Category with name '{dto.Name}' already exists");

            var category = _mapper.Map<Category>(dto);
            var created = await _repository.AddAsync(category);
            return await MapToDtoWithCount(created);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;
            return await MapToDtoWithCount(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            var dtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                dtos.Add(await MapToDtoWithCount(category));
            }

            return dtos;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            // Vérifier si le nouveau nom existe déjà (si différent)
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != existing.Name)
            {
                var existingWithName = await _repository.GetByNameAsync(dto.Name);
                if (existingWithName != null)
                    throw new ArgumentException($"Category with name '{dto.Name}' already exists");
            }

            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return await MapToDtoWithCount(updated);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm)
        {
            var categories = await _repository.GetAllAsync();
            var filtered = categories
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var dtos = new List<CategoryDto>();
            foreach (var category in filtered)
            {
                dtos.Add(await MapToDtoWithCount(category));
            }

            return dtos;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<CategoryDto> MapToDtoWithCount(Category category)
        {
            var dto = _mapper.Map<CategoryDto>(category);
            dto.ArticleCount = await _repository.GetArticleCountAsync(category.Id);
            return dto;
        }
    }
}