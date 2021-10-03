using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace UCare.Models.AppSettingsModel
{
   public class AspNetRoleClaims
    {
        public IdentityRole Role { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
