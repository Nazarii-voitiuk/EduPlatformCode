using EduCodePlatform.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("EditorSetting")]
    public class EditorSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EditorSettingId")]
        public int EditorSettingId { get; set; }

        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        [Column("Theme")]
        public string Theme { get; set; }

        [Column("TabSize")]
        public int TabSize { get; set; }

        // Можна зберігати дату оновлення
        public DateTime UpdatedAt { get; set; }
    }
}
