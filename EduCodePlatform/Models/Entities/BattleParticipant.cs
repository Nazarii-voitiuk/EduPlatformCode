using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCodePlatform.Data.Entities
{
    [Table("BattleParticipant")]
    public class BattleParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BattleParticipantId")]
        public int BattleParticipantId { get; set; }

        [Column("BattleId")]
        public int BattleId { get; set; }

        [ForeignKey(nameof(BattleId))]
        public CodingBattle CodingBattle { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Column("Score")]
        public int Score { get; set; }

        [Column("JoinedAt")]
        public DateTime JoinedAt { get; set; }
    }
}
