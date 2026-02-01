using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureAPI_JMT.Models; // Pour ApplicationUser, AddRoleModel
using SecureAPI_JWT.Helpers;
using SecureAPI_JWT.Models; // Pour tous les autres modèles
using SecureAPI_JWT.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureAPI_JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _emailService = emailService;
            _logger = logger;
        }

        // --- INSCRIPTION (par défaut Client) ---
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            _logger.LogInformation("[REGISTER] Début de l'inscription pour {Email}", model.Email);

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                _logger.LogWarning("[REGISTER] Email déjà enregistré: {Email}", model.Email);
                return new AuthModel { Message = "Email is already registered!" };
            }

            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                _logger.LogWarning("[REGISTER] Username déjà pris: {Username}", model.Username);
                return new AuthModel { Message = "Username is already registered!" };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("[REGISTER] Création de l'utilisateur: {Username}", model.Username);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("[REGISTER] Échec création utilisateur: {Errors}", errors);
                return new AuthModel { Message = errors };
            }

            _logger.LogInformation("[REGISTER] Utilisateur créé avec ID: {UserId}", user.Id);

            // Créer le rôle "Client" s'il n'existe pas
            if (!await _roleManager.RoleExistsAsync("Client"))
            {
                _logger.LogInformation("[REGISTER] Création du rôle 'Client'");
                await _roleManager.CreateAsync(new IdentityRole("Client"));
            }

            // Attribuer le rôle "Client" par défaut
            await _userManager.AddToRoleAsync(user, "Client");
            _logger.LogInformation("[REGISTER] Rôle 'Client' attribué à {Username}", user.UserName);

            // Créer le rôle "ResponsableSAV" s'il n'existe pas
            if (!await _roleManager.RoleExistsAsync("ResponsableSAV"))
            {
                _logger.LogInformation("[REGISTER] Création du rôle 'ResponsableSAV'");
                await _roleManager.CreateAsync(new IdentityRole("ResponsableSAV"));
            }

            // Envoi d'email
            var emailModel = new EmailModel
            {
                To = user.Email,
                Subject = "Bienvenue dans notre système SAV",
                Body = $@"
                    <h2>Bonjour {user.FirstName} {user.LastName},</h2>
                    <p>Votre compte a été créé avec succès en tant que Client.</p>
                    <p><strong>Email:</strong> {user.Email}</p>
                    <p><strong>Nom d'utilisateur:</strong> {user.UserName}</p>
                    <p>Connectez-vous à l'adresse : http://localhost:5184</p>
                "
            };

            try
            {
                await _emailService.SendEmailAsync(emailModel);
                _logger.LogInformation("[EMAIL] Email envoyé avec succès à {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EMAIL ERROR] Erreur lors de l'envoi à {Email}", user.Email);
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Message = "Registration successful!",
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "Client" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        // --- LOGIN ---
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.Message = "Login successful!";

            return authModel;
        }

        // --- AJOUTER RÔLE ---
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";
        }

        // --- GET ALL USERS ---
        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userResponses = new List<UserResponse>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userResponses.Add(new UserResponse
                    {
                        Id = user.Id ?? string.Empty,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        PhoneNumber = user.PhoneNumber ?? string.Empty,
                        Roles = roles.ToList(),
                        CreatedAt = user.CreatedAt
                    });
                }

                return userResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GET ALL USERS] Erreur lors de la récupération des utilisateurs");
                throw;
            }
        }

        // --- CRÉER UN UTILISATEUR (ResponsableSAV seulement) ---
        public async Task<AuthModel> CreateUserAsync(CreateUserModel model)
        {
            _logger.LogInformation("[CREATE USER] Création d'un nouvel utilisateur par ResponsableSAV pour {Email}", model.Email);

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            // Vérifier si le rôle existe
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return new AuthModel { Message = $"Role '{model.Role}' does not exist!" };

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            // Attribuer le rôle spécifié
            await _userManager.AddToRoleAsync(user, model.Role);

            // Envoyer email
            var emailModel = new EmailModel
            {
                To = user.Email,
                Subject = "Votre compte a été créé",
                Body = $@"
                    <h2>Bonjour {user.FirstName} {user.LastName},</h2>
                    <p>Votre compte a été créé avec le rôle: {model.Role}</p>
                    <p><strong>Email:</strong> {user.Email}</p>
                    <p><strong>Nom d'utilisateur:</strong> {user.UserName}</p>
                "
            };

            try
            {
                await _emailService.SendEmailAsync(emailModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EMAIL ERROR] Erreur lors de l'envoi à {Email}", user.Email);
            }

            return new AuthModel
            {
                Message = $"User created successfully with role '{model.Role}'",
                Email = user.Email,
                IsAuthenticated = false,
                Username = user.UserName
            };
        }

        // --- METTRE À JOUR UN UTILISATEUR ---
        public async Task<string> UpdateUserAsync(string userId, UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";

            // Mettre à jour les champs
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.Username))
                user.UserName = model.Username;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return string.Empty;
        }

        // --- SUPPRIMER UN UTILISATEUR ---
        public async Task<string> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User not found";

            // Empêcher la suppression d'un ResponsableSAV s'il est le dernier
            if (await _userManager.IsInRoleAsync(user, "ResponsableSAV"))
            {
                var allResponsableSAVs = await _userManager.GetUsersInRoleAsync("ResponsableSAV");
                if (allResponsableSAVs.Count <= 1)
                    return "Cannot delete the last ResponsableSAV";
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return string.Empty;
        }

        // --- GET USER BY ID ---
        public async Task<UserResponse> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse
            {
                Id = user.Id ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}