using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduCodePlatform.Models.Entities;

namespace EduCodePlatform.Data.Entities
{
    [Table("TaskSubmission")]
    public class TaskSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TaskSubmissionId")]
        public int TaskSubmissionId { get; set; }

        // Зовнішній ключ на Task
        [Column("TaskId")]
        public int TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public CodingTask Task { get; set; }

        // Зовнішній ключ на CodeSubmission
        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        [ForeignKey(nameof(CodeSubmissionId))]
        public CodeSubmission CodeSubmission { get; set; }

        [Column("Score")]
        public float Score { get; set; }

        [Column("SubmittedAt")]
        public DateTime SubmittedAt { get; set; }

        [Column("PassedAllTests")]
        public bool PassedAllTests { get; set; }
    }
}
