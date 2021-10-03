using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.Entities
{
    public class Videotelephony
    {
        public Guid VideotelephonyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
