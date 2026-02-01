using BlazorApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public interface IRoleService
    {
        Task<List<RoleModel>> GetAllRolesAsync();
        Task<string> CreateRoleAsync(string roleName);
        Task<string> DeleteRoleAsync(string roleName);
        Task<List<string>> GetUsersInRoleAsync(string roleName);
        Task<string> TestConnectionAsync(); // Pour debug

    }
}