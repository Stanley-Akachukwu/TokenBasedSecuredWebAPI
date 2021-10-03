using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UCare.Models.UserViewModels
{
    public class UserClaimsViewModel: UserManagerResponse
    {
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
}
