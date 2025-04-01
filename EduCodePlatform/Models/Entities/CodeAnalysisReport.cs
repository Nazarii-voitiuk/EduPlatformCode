using EduCodePlatform.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("CodeAnalysisReport")]
    public class CodeAnalysisReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ReportId")]
        public int ReportId { get; set; }

        [Required]
        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        [ForeignKey(nameof(CodeSubmissionId))]
        public CodeSubmission CodeSubmission { get; set; }

        [Column("AnalysisDate")]
        public DateTime AnalysisDate { get; set; }

        [Column("StyleScore")]
        public float StyleScore { get; set; }

        [Column("PerformanceScore")]
        public float PerformanceScore { get; set; }

        [Column("SecurityScore")]
        public float SecurityScore { get; set; }

        [Column("ReportDetails", TypeName = "text")]
        public string ReportDetails { get; set; }
    }
}
