using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class UserViewModel: UserManagerResponse
    {
        public Guid UserId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string LastName { get; set; }
        [StringLength(50, MinimumLength = 10)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 10)]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [StringLength(120)]
        public string Address { get; set; }
        [Required]
        [StringLength(50)]
        public string StateOfOrigin { get; set; }
        [Required]
        [StringLength(50)]
        public string CountryOfOrigin { get; set; }
        [Required]
        [StringLength(100)]
        public string Village { get; set; }
        [Required]
        [StringLength(50)]
        public string ResidentState { get; set; }
        [Required]
        [StringLength(50)]
        public string ResidentCountry { get; set; }
        public int Age { get; set; } = 2;
        public DateTime RegDate { get; set; } = DateTime.MinValue;
    }
}
