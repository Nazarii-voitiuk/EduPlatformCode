using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("PullRequest")]
    public class PullRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("PullRequestId")]
        public int PullRequestId { get; set; }

        [Column("ProjectId")]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description", TypeName = "text")]
        public string Description { get; set; }

        [Column("Status")]
        public string Status { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
