using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Data.Concrete
{
    public class AdminRepository : IAdminRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISettingsRepository _settingsRepository;
        public AdminRepository(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ISettingsRepository settingsRepository)
        {
            _roleManager = roleManager;
            _settingsRepository = settingsRepository;
            _userManager = userManager;
        }
        public async Task<IdentityResult> CreateRoleAsync(RoleViewModel model)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = model.RoleName
            };
            return await _roleManager.CreateAsync(identityRole);
        }
        public List<IdentityRole> ListRolesAsync()
        {
            return _roleManager.Roles.ToList();
        }
        public async Task<RoleViewModel> GetRoleIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var model = new RoleViewModel();
            if (role == null)
            {
                return new RoleViewModel
                {
                    ResponseMessage = $"Role with Id = {roleId} cannot be found",
                Succeeded = false,
                    Errors = new List<string> { "Confirm password doesn't match the password" }
                };

            }
            model = new RoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                ResponseMessage = $"Successfully.",
                Succeeded = true,
                ResponseCode = StatusCodes.Status200OK
            };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return model;
        }
        public async Task<RoleViewModel> UpdateRoleAsync(RoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
             model = new RoleViewModel();
            if (role == null)
            {
                model.ResponseMessage = $"Role with Id = {model.Id} cannot be found";
                return model;
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    model = new RoleViewModel
                    {
                        Id = role.Id,
                        RoleName = role.Name
                    };
                    foreach (var user in _userManager.Users)
                    {
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            model.Users.Add(user.UserName);
                        }
                    }
                }
                return model;
            }
        }

        public async Task<List<UserRoleViewModel>> GetUsersInRoleAsync(string roleId)
        {

            var role = await _roleManager.FindByIdAsync(roleId);

            var model = new List<UserRoleViewModel>();
            if (role == null)
            {
                model.Add(new UserRoleViewModel { ResponseMessage = $"Role with Id = {roleId} cannot be found",Succeeded=false });
                return model;
            }


            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return model;
        }

        public async Task<List<UserRoleViewModel>> UpdateUsersInRoleAsync(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

             model = new List<UserRoleViewModel>();
            if (role == null)
            {
                model.Add(new UserRoleViewModel { ResponseMessage = $"Role with Id = {roleId} cannot be found", Succeeded = false });
                return model;
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                    {
                        model = new List<UserRoleViewModel>();

                        foreach (var userEdit in _userManager.Users)
                        {
                            var userRoleViewModel = new UserRoleViewModel
                            {
                                UserId = userEdit.Id,
                                UserName = userEdit.UserName
                            };

                            if (await _userManager.IsInRoleAsync(userEdit, role.Name))
                            {
                                userRoleViewModel.IsSelected = true;
                            }
                            else
                            {
                                userRoleViewModel.IsSelected = false;
                            }

                            model.Add(userRoleViewModel);
                            return model;
                        }
                    }
                }

            }
            return null;
        }

        public async Task<UserRolesViewModelResponse> GetRolesForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRolesViewModels = new List<UserRolesViewModel>();
            var response = new UserRolesViewModelResponse();
            if (user == null)
            {
                response.ResponseCode = StatusCodes.Status400BadRequest;
                response.ResponseMessage = $"User with Id = {userId} cannot be found";
                return response;
            }

            foreach (var role in _roleManager.Roles)
            {
                var manageUserRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    manageUserRolesViewModel.IsSelected = true;
                }
                else
                {
                    manageUserRolesViewModel.IsSelected = false;
                }

                userRolesViewModels.Add(manageUserRolesViewModel);
            }
            response.UserRolesViewModels = userRolesViewModels;
            response.ResponseCode = StatusCodes.Status200OK;
            response.ResponseMessage = $"Succesful.";
            response.Succeeded = true;
            return response;
        }

        public async Task<UserRolesViewModelResponse> UpdateRolesForUserAsync(List<UserRolesViewModel> userRolesViewModels, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            userRolesViewModels = new List<UserRolesViewModel>();
            var response = new UserRolesViewModelResponse();
            if (user == null)
            {
                response.ResponseCode = StatusCodes.Status400BadRequest;
                response.ResponseMessage = $"User with Id = {userId} cannot be found";
                response.Errors = new List<string>() { $"User with Id = {userId} cannot be found" };
                return response;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                return new UserRolesViewModelResponse() {
                    ResponseMessage = result.Errors.Select(e => e.Description).FirstOrDefault(),
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description),
                    ResponseCode = StatusCodes.Status204NoContent,
                };
            }

            result = await _userManager.AddToRolesAsync(user,
        userRolesViewModels.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                response.ResponseCode = StatusCodes.Status500InternalServerError;
                response.ResponseMessage = $"User with Id = {userId} cannot be found";
                response.Errors = new List<string>() { $"User with Id = {userId} cannot be found" };
                return response;
            }
            response.ResponseCode = StatusCodes.Status200OK;
            response.ResponseMessage = $"User roles were updated successfully";
            response.Succeeded = true;
            return response;
        }

        public async Task<UserClaimsViewModel> GetUserClaimsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var model = new UserClaimsViewModel();
            if (user == null)
            {
                model.ResponseMessage = $"User with Id = {userId} cannot be found"; model.Succeeded = false;
               return model;
            }
            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            model = new UserClaimsViewModel
            {
                UserId = userId
            };
            List<Claim> allClaims = _settingsRepository.GetClaimsStore();  

            foreach (Claim claim in allClaims)
            {
                
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return model;
        }

        public async Task<UserClaimsViewModel> UpdateUserClaimsAsync(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                model = new UserClaimsViewModel();
                model.ResponseMessage = $"User with Id = {model.UserId} cannot be found";
                return model;
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                model = new UserClaimsViewModel();
                model.ResponseMessage = $"Cannot remove user existing claims";
                return model;
            }

            result = await _userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                model = new UserClaimsViewModel();
                model.ResponseMessage = $"Cannot add selected claims to user";
                return model;
            }
            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            model = new UserClaimsViewModel
            {
                UserId = model.UserId
            };
            List<Claim> allClaims = _settingsRepository.GetClaimsStore();
            foreach (Claim claim in allClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return model;
        }

        public async Task<UserManagerResponse> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var reponse = new UserManagerResponse();
            if (role == null)
            {
                reponse.ResponseMessage = $"Role with Id = {id} cannot be found";
                reponse.Succeeded = false;
                return reponse;
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        reponse.ResponseMessage = $"Role with Id = {id} has been removed successfully";
                        reponse.Succeeded = false;
                        return reponse;
                    }
                }
                catch (DbUpdateException ex)
                {
                    reponse.ResponseMessage = $"{role.Name} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete-"+ ex.StackTrace;
                    reponse.Succeeded = false;
                }
                return reponse;
            }
        }
    }
}
