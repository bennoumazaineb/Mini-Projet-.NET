using BlazorApp1.Models;

namespace BlazorApp1.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse?> CreateCategoryAsync(CategoryCreateDto category);
        Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        Task<List<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> UpdateCategoryAsync(int id, CategoryUpdateDto category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<CategoryResponse>> SearchCategoriesAsync(string term);
    }
}
