using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Services.Abstract
{
   public interface IRoleService
    {
        Task<List<IdentityRole>> GetRoles(string accessToken);
        Task<RoleViewModel> GetRole(string id, string accessToken);
        Task<RoleViewModel> UpdateRole(RoleViewModel model, string accessToken);
        Task<IdentityResult> CreateRole(RoleViewModel model, string accessToken);
        Task DeleteIdentityRole(string id,string accessToken);
    }
}
