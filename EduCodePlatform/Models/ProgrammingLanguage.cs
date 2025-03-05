using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EduCodePlatform.Models
{
    public class ProgrammingLanguage
    {
        [Key]
        public int LanguageId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<CodeSubmission> CodeSubmissions { get; set; }
    }
}
