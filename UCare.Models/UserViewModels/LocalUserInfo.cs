using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace UCare.Models.UserViewModels
{
    public class LocalUserInfo
    {
        public string AccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
    }
}
