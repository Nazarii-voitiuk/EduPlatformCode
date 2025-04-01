using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Column("UserId")]
        public override string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
