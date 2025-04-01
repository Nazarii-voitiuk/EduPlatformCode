using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduCodePlatform.Models.Entities;

namespace EduCodePlatform.Data.Entities
{
    [Table("CodeTestResult")]
    public class CodeTestResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TestResultId")]
        public int TestResultId { get; set; }

        [Required]
        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        [ForeignKey(nameof(CodeSubmissionId))]
        public CodeSubmission CodeSubmission { get; set; }

        [Column("TestName")]
        public string TestName { get; set; }

        [Column("Passed")]
        public bool Passed { get; set; }

        [Column("Output", TypeName = "text")]
        public string Output { get; set; }

        [Column("RunAt")]
        public DateTime RunAt { get; set; }
    }
}
