using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Models.Entities
{
    [Table("AppUser")]
    public class AppUser : IdentityUser
    {
        // PK: UserId (varchar)
        [Key]
        [Column("UserId")]
        public string UserId { get; set; }

        [Required]
        [Column("UserName")]
        public string UserName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
