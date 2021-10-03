using System;
using System.ComponentModel.DataAnnotations;

namespace UCare.Models.Entities
{
    public class Language
    {
        public Guid LanguageId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string LanguageName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string TribeOrRace { get; set; }
        public int CompetitionId { get; set; } = 0;
        public Competition Competition { get; set; }
    }
}