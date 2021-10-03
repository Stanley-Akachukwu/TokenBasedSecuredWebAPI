using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class UserRolesViewModel
    {
        // model for how to add or remove roles for a given user
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
   
}
