using EduCodePlatform.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Models
{
    public class CodeSubmission
    {
        [Key]
        public int CodeSubmissionId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string CodeText { get; set; }

        [Required]
        [ForeignKey("ProgrammingLanguage")]
        public int LanguageId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Навігаційна властивість для зв’язку з ProgrammingLanguage
        public virtual ProgrammingLanguage ProgrammingLanguage { get; set; }
    }
}
