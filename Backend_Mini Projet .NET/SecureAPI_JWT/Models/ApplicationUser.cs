using Microsoft.AspNetCore.Identity;
using System;

namespace SecureAPI_JMT.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; } // Rend nullable
        public string? LastName { get; set; }  // Rend nullable
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}