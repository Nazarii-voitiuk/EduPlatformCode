using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduCodePlatform.Models.Entities;

namespace EduCodePlatform.Data.Entities
{
    [Table("CodeSubmissionHistory")]
    public class CodeSubmissionHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("HistoryId")]
        public int HistoryId { get; set; }

        [Required]
        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        [ForeignKey(nameof(CodeSubmissionId))]
        public CodeSubmission CodeSubmission { get; set; }

        [Column("CodeSnapshot", TypeName = "text")]
        public string CodeSnapshot { get; set; }

        [Column("ChangedAt")]
        public DateTime ChangedAt { get; set; }
    }
}
