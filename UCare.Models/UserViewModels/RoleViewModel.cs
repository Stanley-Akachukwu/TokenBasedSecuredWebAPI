using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class RoleViewModel: UserManagerResponse
    {
        public RoleViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
