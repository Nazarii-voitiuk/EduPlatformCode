using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("ProgrammingLanguage")]
    public class ProgrammingLanguage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("LanguageId")]
        public int LanguageId { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }
    }
}
