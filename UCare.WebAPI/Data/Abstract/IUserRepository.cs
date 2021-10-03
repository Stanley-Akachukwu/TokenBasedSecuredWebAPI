using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;

namespace UCare.WebAPI.Data.Abstract
{
   public interface IUserRepository
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        List<IdentityUser> GetUsersAsync();
        Task<EditUserViewModel> GetUserByIdAsync(string id);
        Task<EditUserViewModel> UpdateUserAsync(EditUserViewModel model);
        Task<UserManagerResponse> DeleteUserAsync(string id);
    }
}
