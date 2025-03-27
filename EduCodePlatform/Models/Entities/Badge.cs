using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("Badge")]
    public class Badge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BadgeId")]
        public int BadgeId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description", TypeName = "text")]
        public string Description { get; set; }

        [Column("IconUrl")]
        public string IconUrl { get; set; }

        [Column("Criteria", TypeName = "text")]
        public string Criteria { get; set; }
    }
}
