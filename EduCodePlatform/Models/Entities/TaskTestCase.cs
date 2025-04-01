using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("TaskTestCase")]
    public class TaskTestCase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestCaseId { get; set; }

        [Column("TaskId")]
        public int TaskId { get; set; }
        public CodingTask Task { get; set; }

        [Column("HtmlRules", TypeName = "text")]
        public string HtmlRules { get; set; }

        [Column("CssRules", TypeName = "text")]
        public string CssRules { get; set; }

        [Column("InputData", TypeName = "text")]
        public string InputData { get; set; }

        [Column("ExpectedJsOutput", TypeName = "text")]
        public string ExpectedJsOutput { get; set; }

        public int Points { get; set; }

        public int TimeLimitSeconds { get; set; } = 2;
    }
}
