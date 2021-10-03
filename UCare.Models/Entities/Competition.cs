using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.Entities
{
    public class Competition
    {
        public Guid CompetitionId { get; set; }
        [Required]
        [StringLength(120)]
        public string Title { get; set; }
        [Required]
        [StringLength(20)]
        public string Moniker { get; set; }
        public Videotelephony Videotelephony { get; set; }
        public int VideotelephonyId { get; set; }
        public int ViewCapacity { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;
        public Challenger Challenger { get; set; }
        public int ChallengerId { get; set; }
        public Offense Offense { get; set; }
        public int OffenseId { get; set; }
        public Moderator Moderator { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
        public ICollection<Judge> Judges { get; set; }
        public int ModeratorId { get; set; }
        public ICollection<Spectator> Spectators { get; set; }
        public Competition()
        {
            Spectators = new Collection<Spectator>();
            Judges = new Collection<Judge>();
        }
    }
}
