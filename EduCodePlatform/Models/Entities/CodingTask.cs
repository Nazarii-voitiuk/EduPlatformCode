using EduCodePlatform.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("Task")]
    public class CodingTask 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TaskId")]
        public int TaskId { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Description", TypeName = "text")]
        public string Description { get; set; }

        [Column("DifficultyId")]
        public int DifficultyId { get; set; }

        [ForeignKey(nameof(DifficultyId))]
        public TaskDifficulty Difficulty { get; set; }

        [Column("IsAIgenerated")]
        public bool IsAIgenerated { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public AppUser CreatedByUser { get; set; }
    }
}
