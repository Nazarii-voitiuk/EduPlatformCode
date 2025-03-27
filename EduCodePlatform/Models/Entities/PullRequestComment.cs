using EduCodePlatform.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("PullRequestComment")]
    public class PullRequestComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CommentId")]
        public int CommentId { get; set; }

        [Column("PullRequestId")]
        public int PullRequestId { get; set; }

        [ForeignKey(nameof(PullRequestId))]
        public PullRequest PullRequest { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        [Column("CommentText", TypeName = "text")]
        public string CommentText { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
