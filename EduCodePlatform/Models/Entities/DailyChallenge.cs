using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("DailyChallenge")]
    public class DailyChallenge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DailyChallengeId")]
        public int DailyChallengeId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description", TypeName = "text")]
        public string Description { get; set; }

        [Column("ChallengeDate")]
        public DateTime ChallengeDate { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ApplicationUser CreatedByUser { get; set; }
    }
}
