using BlazorApp1.Models;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> GetUserByIdAsync(string id);
    Task<string> UpdateUserAsync(string id, UpdateUserModel model);
    Task<string> DeleteUserAsync(string id);
    Task<string> AddRoleAsync(AddRoleModel model);
    Task<string> CreateUserAsync(CreateUserModel model);
    Task<List<UserResponse>> GetClientsAsync();
    Task<List<UserResponse>> GetResponsablesAsync();
    Task<List<UserResponse>> SearchUsersAsync(string email = "", string name = "");
}
