using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureAPI_JMT.Models;
using SecureAPI_JWT.Models;
using SecureAPI_JWT.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }

        // --- Register ---
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        // --- Login ---
        [HttpPost("login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }


    }
        // --- Modèle de réponse pour les clients ---
        public class ClientResponse
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public List<string> Roles { get; set; } = new List<string>();
            public DateTime CreatedAt { get; set; }
        }
    }
