using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Services.Abstract
{
   public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string accessToken);

        Task<UserManagerResponse> ForgetPasswordAsync(string email, string accessToken);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<IEnumerable<IdentityUser>> GetUsersAsync(string accessToken);
        Task<EditUserViewModel> GetUserByIdAsync(string id, string accessToken);
        Task<EditUserViewModel> UpdateUserAsync(EditUserViewModel model, string accessToken);
        Task DeleteUserAsync(string id, string accessToken);
        Task<UserRolesViewModelResponse> GetRolesForUser(string id, string accessToken);
        Task<UserRolesViewModelResponse> UpdateRolesForUser(string id, List<UserRolesViewModel> userRoles, string accessToken);
    }
}
