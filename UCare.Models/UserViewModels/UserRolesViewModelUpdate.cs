using System;
using System.Collections.Generic;
using System.Text;

namespace UCare.Models.UserViewModels
{
  public  class UserRolesViewModelUpdate
    {
        public List<UserRolesViewModel> EditedUserRoles { get; set; }
        public string UserId { get; set; }
    }
}
