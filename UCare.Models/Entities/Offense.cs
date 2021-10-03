using System;
using System.ComponentModel.DataAnnotations;

namespace UCare.Models.Entities
{
    public class Offense
    {
        public Guid OffenseId { get; set; }
        [Required]
        [StringLength(120)]
        public string Title { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string  OffensiveVideoFilePath { get; set; }
        public int CompetitionId { get; set; } = 0;
        public Competition Competition { get; set; }
        public DateTime OffensiveDate { get; set; } = DateTime.MinValue;
    }
}