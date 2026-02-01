using SecureAPI_JMT.Models;
using SecureAPI_JWT.Models; // ou le namespace de vos modèles
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureAPI_JWT.Services
{
    public interface IAuthService
    {
        // Inscription (pour tous)
        Task<AuthModel> RegisterAsync(RegisterModel model);

        // Connexion
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);

        // Ajouter rôle
        Task<string> AddRoleAsync(AddRoleModel model);

        // Récupérer tous les utilisateurs
        Task<List<UserResponse>> GetAllUsersAsync();

        // CRUD - ResponsableSAV seulement
        Task<AuthModel> CreateUserAsync(CreateUserModel model);
        Task<string> UpdateUserAsync(string userId, UpdateUserModel model);
        Task<string> DeleteUserAsync(string userId);
        Task<UserResponse> GetUserByIdAsync(string userId);
    }
}