using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EduCodePlatform.Models.Identity;

namespace EduCodePlatform.Models.Entities
{
    [Table("CodeSubmission")]
    public class CodeSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CodeSubmissionId")]
        public int CodeSubmissionId { get; set; }

        // Зовнішній ключ на AppUser (Identity)
        [Required]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // Окремі поля
        [Column("HtmlCode", TypeName = "text")]
        public string HtmlCode { get; set; }

        [Column("CssCode", TypeName = "text")]
        public string CssCode { get; set; }

        [Column("JsCode", TypeName = "text")]
        public string JsCode { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
