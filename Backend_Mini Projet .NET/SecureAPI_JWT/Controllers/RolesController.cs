// Controllers/RolesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SecureAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: api/roles
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles
                .Select(r => new { r.Id, r.Name })
                .ToList();
            return Ok(roles);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Le nom du rôle est requis");

            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Le rôle existe déjà");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return Ok($"Rôle '{roleName}' créé avec succès");

            return BadRequest(result.Errors);
        }

        // DELETE: api/roles/{roleName}
        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Rôle non trouvé");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok($"Rôle '{roleName}' supprimé avec succès");

            return BadRequest(result.Errors);
        }
    }
}