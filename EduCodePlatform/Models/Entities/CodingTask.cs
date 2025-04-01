using EduCodePlatform.Models.Identity;
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
        public TaskDifficulty Difficulty { get; set; }

        [Column("IsAIgenerated")]
        public bool IsAIgenerated { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; }

        // Можна зберігати референс-код, якщо хочемо
        [Column("ReferenceHtml", TypeName = "text")]
        public string ReferenceHtml { get; set; }

        [Column("ReferenceCss", TypeName = "text")]
        public string ReferenceCss { get; set; }

        [Column("ReferenceJs", TypeName = "text")]
        public string ReferenceJs { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ApplicationUser CreatedByUser { get; set; }
    }
}
