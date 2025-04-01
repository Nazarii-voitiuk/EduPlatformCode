using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("DailyChallengeSubmission")]
    public class DailyChallengeSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DailyChallengeSubmissionId")]
        public int DailyChallengeSubmissionId { get; set; }

        [Column("DailyChallengeId")]
        public int DailyChallengeId { get; set; }

        [ForeignKey(nameof(DailyChallengeId))]
        public DailyChallenge DailyChallenge { get; set; }

        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        [ForeignKey(nameof(CodeSubmissionId))]
        public CodeSubmission CodeSubmission { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Column("SubmittedAt")]
        public DateTime SubmittedAt { get; set; }

        [Column("Score")]
        public int Score { get; set; }
    }
}
