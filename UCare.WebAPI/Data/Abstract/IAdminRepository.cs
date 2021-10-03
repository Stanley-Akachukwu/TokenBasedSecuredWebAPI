using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCare.Models.UserViewModels;

namespace UCare.WebAPI.Data.Abstract
{
  public  interface IAdminRepository
    {
        Task<IdentityResult> CreateRoleAsync(RoleViewModel model);
        List<IdentityRole> ListRolesAsync();
        Task<RoleViewModel> GetRoleIdAsync(string id);
        Task<RoleViewModel> UpdateRoleAsync(RoleViewModel model);
       Task<List<UserRoleViewModel>> GetUsersInRoleAsync(string roleId);
        Task<List<UserRoleViewModel>> UpdateUsersInRoleAsync(List<UserRoleViewModel> model, string roleId);
        Task<UserRolesViewModelResponse> GetRolesForUserAsync(string userId);
        Task<UserRolesViewModelResponse> UpdateRolesForUserAsync(List<UserRolesViewModel> model, string userId);
        Task<UserClaimsViewModel> GetUserClaimsAsync(string userId);
        Task<UserClaimsViewModel> UpdateUserClaimsAsync(UserClaimsViewModel model);
        Task<UserManagerResponse> DeleteRoleAsync(string id);
    }
}
