using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("TaskTestCase")]
    public class TaskTestCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TestCaseId")]
        public int TestCaseId { get; set; }

        // Зовнішній ключ на Task
        [Column("TaskId")]
        public int TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public CodingTask Task { get; set; }

        [Column("Input", TypeName = "text")]
        public string Input { get; set; }

        [Column("ExpectedOutput", TypeName = "text")]
        public string ExpectedOutput { get; set; }

        [Column("Points")]
        public int Points { get; set; }
    }
}
