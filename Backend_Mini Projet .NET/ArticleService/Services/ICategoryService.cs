using ArticleService.Models.DTOs;

namespace ArticleService.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto);
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryDto>> SearchCategoriesAsync(string searchTerm);
        Task<bool> CategoryExistsAsync(int id);
    }
}