using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform   .Models
{
    public class EditorSetting
    {
        [Key]
        public int EditorSettingId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Theme { get; set; } // Наприклад, "dark" або "light"

        [Required]
        public int TabSize { get; set; }
    }
}
