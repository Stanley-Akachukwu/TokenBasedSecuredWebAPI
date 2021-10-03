using System;
using System.Collections.Generic;
using System.Text;

namespace UCare.Models.UserViewModels
{
    public class UserRolesViewModelResponse : UserManagerResponse
    {
        public List<UserRolesViewModel> UserRolesViewModels { get; set; }
    }
}
