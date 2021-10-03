using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class UserRoleViewModel: UserManagerResponse
    {
        //model to add or remove users from a given role
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
