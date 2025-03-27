using EduCodePlatform.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("Project")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProjectId")]
        public int ProjectId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description", TypeName = "text")]
        public string Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("OwnerId")]
        public string OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public AppUser Owner { get; set; }
    }
}
