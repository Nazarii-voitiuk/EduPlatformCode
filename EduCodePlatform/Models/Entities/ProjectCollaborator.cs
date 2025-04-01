using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("ProjectCollaborator")]
    public class ProjectCollaborator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProjectCollaboratorId")]
        public int ProjectCollaboratorId { get; set; }

        [Column("ProjectId")]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Column("Role")]
        public string Role { get; set; }
    }
}
