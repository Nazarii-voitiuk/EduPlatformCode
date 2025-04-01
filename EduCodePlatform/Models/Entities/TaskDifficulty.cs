using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("TaskDifficulty")]
    public class TaskDifficulty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DifficultyId { get; set; }

        [Column("DifficultyName")]
        public string DifficultyName { get; set; }
    }
}
