using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("UserBadge")]
    public class UserBadge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserBadgeId")]
        public int UserBadgeId { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // Зовнішній ключ на Badge
        [Column("BadgeId")]
        public int BadgeId { get; set; }

        [ForeignKey(nameof(BadgeId))]
        public Badge Badge { get; set; }

        [Column("AwardedAt")]
        public DateTime AwardedAt { get; set; }
    }
}
