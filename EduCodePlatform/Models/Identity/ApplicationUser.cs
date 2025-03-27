using Microsoft.AspNetCore.Identity;
using System;

namespace EduCodePlatform.Models.Identity
{
    // Наш користувач із додатковим полем CreatedAt
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
