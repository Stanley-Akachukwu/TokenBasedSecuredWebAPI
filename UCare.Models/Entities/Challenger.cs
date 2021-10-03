using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.Entities
{
    public class Challenger
    {
        public Guid ChallengerId { get; set; }
        [Required]
        [StringLength(120)]
        public string Title { get; set; }
        public User User { get; set; }
        [Required]
        [StringLength(50)]
        public string ChallengeVideoFilePath { get; set; }
        public int CompetitionId { get; set; } = 0;
        public Competition Competition { get; set; }
        public DateTime ChallengDate { get; set; } = DateTime.MinValue;

    }
}
