using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ucare.WebUI.Models;
using Ucare.WebUI.Services.Abstract;
using UCare.Models.AppSettingsModel;
using UCare.Models.UserViewModels;


namespace Ucare.WebUI.Pages.Admin
{
    public class UsersBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; }
        public bool isBusy = false;
        public string message = string.Empty;
        public bool activeComponent = true;
        public bool identityUsersListComponent = false;
        public bool userEditorComponent = false;
        public bool manageUserRolesComponent = false;
        public AlertMessageType messageType = AlertMessageType.Success;
        [Parameter]
        public string pageHeaderText { get; set; }
        [Parameter]
        public EditUserViewModel editUserViewModel { get; set; } = new EditUserViewModel();
        [Inject]
        public IToastService _toastService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        [Inject]
        public IUserService _userService { get; set; }
        [Inject]
        public IRoleService _roleService { get; set; }
        [Parameter]
        public IEnumerable<IdentityUser> registeredUsers { get; set; }
        [Parameter]
        public LocalUserInfo localUserInfo { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject]
        private IAuthorizationService _authorizationService { get; set; }
        public List<string> claims { get; set; }

        public IList<string> roles { get; set; }
        [Parameter]
        public List<UserRolesViewModel> userRoles { get; set; } = new List<UserRolesViewModel>();

        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            identityUsersListComponent = true;
            userEditorComponent = false;
            manageUserRolesComponent = false;
        var user = (await authenticationStateTask).User;
            localUserInfo = await _localStorageService.GetItemAsync<LocalUserInfo>("User");
            try
            {
                if (user.Identity.IsAuthenticated)
                {
                    registeredUsers = (await _userService.GetUsersAsync(localUserInfo.AccessToken)).ToList();
                }
                if (user.Identity.IsAuthenticated)
                {
                    // Perform an action only available to authenticated (signed-in) users.
                }

                if (user.IsInRole("admin"))
                {
                    // Perform an action only available to users in the 'admin' role.
                }

                if ((await _authorizationService.AuthorizeAsync(user, Policies.IsAdmin))
                    .Succeeded)
                {
                    // Perform an action only available to users satisfying the 
                    // 'content-editor' policy.
                }
            }
            catch (Exception exp)
            {
                isBusy = false;
                if (exp.Message.Contains("403")|| exp.Message.Contains("401"))
                {
                    _toastService.ShowWarning("Sorry, you do not have access to the requested resource!");
                    _navigationManager.NavigateTo("/admin");
                }
            }
        }
        protected async Task RegisteredUserDeleted()
        {
            identityUsersListComponent = true;
            userEditorComponent = false;
            manageUserRolesComponent = false;
            registeredUsers = (await _userService.GetUsersAsync(localUserInfo.AccessToken)).ToList();
        }
        protected async Task ShowUserEditor(string id)
        {
            var user = (await authenticationStateTask).User;
            isBusy = true;
            localUserInfo = await _localStorageService.GetItemAsync<LocalUserInfo>("User");
            if ((await _authorizationService.AuthorizeAsync(user, Policies.IsAdmin)).Succeeded)
            {
                if (id != null)
                {
                    editUserViewModel = await _userService.GetUserByIdAsync(id, localUserInfo.AccessToken);
                    if (editUserViewModel != null)
                    {
                        claims = editUserViewModel.Claims;
                        roles = editUserViewModel.Roles;
                    }
                    pageHeaderText = "Edit User";
                    isBusy = false;

                    UserRolesViewModelResponse response = (await _userService.GetRolesForUser(id, localUserInfo.AccessToken));
                    if (response.Succeeded)
                    {
                        userRoles = response.UserRolesViewModels;
                    }
                }
            }
            userEditorComponent = true;
            identityUsersListComponent = false;
            manageUserRolesComponent = false;
            isBusy = false;
        }
        public void ManageUsersRoles(string id)
        {
            pageHeaderText = "Manage " + editUserViewModel.UserName + " User Roles";
            manageUserRolesComponent = true;
            identityUsersListComponent = false;
            userEditorComponent = true;
        }
        public void ManageUsersClaimss(string id)
        {

        }
        protected void CancelUserEdit()
        {
            identityUsersListComponent = true;
            userEditorComponent = false;
            manageUserRolesComponent = false;
            isBusy = false;
        }
        protected async Task HandleUserEditUpdate()
        {
            isBusy = true;
            UserManagerResponse response = null;
            string errors = string.Empty;
            response = await _userService.UpdateUserAsync(editUserViewModel, localUserInfo.AccessToken);
            if (response.ResponseCode == StatusCodes.Status422UnprocessableEntity || response.ResponseCode == StatusCodes.Status409Conflict)
            {
                isBusy = false;
                foreach (var error in response.Errors)
                    errors += error + ". ";
                errors += response.ResponseMessage;
                message = errors;
                messageType = AlertMessageType.Error;
            }
            if (response.ResponseCode == StatusCodes.Status200OK)
            {
                registeredUsers = (await _userService.GetUsersAsync(localUserInfo.AccessToken)).ToList();
                identityUsersListComponent = true;
                userEditorComponent = false;
                manageUserRolesComponent = false;
                isBusy = false;
                _toastService.ShowSuccess(response.ResponseMessage);
            }
        }
        protected void ClosePopupDialog()
        {
            userEditorComponent = true;
            identityUsersListComponent = false;
            manageUserRolesComponent = false;
        }
        protected async Task HandleUserRolesEditUpdate()
        {
            string errors = string.Empty;
            UserRolesViewModelResponse response = (await _userService.UpdateRolesForUser(editUserViewModel.Id, userRoles, localUserInfo.AccessToken));
            if (!response.Succeeded)
            {
                if (response.Errors!=null)
                {
                    foreach (var error in response.Errors)
                        errors += error + ". ";
                    errors += response.ResponseMessage;
                    message = errors; 
                }
                messageType = AlertMessageType.Error;
                userEditorComponent = true;
                identityUsersListComponent = false;
                manageUserRolesComponent = false;
                ShowUserEditor(editUserViewModel.Id);
            }
            if (response.Succeeded)
            {
                userRoles = response.UserRolesViewModels;
                _toastService.ShowSuccess(response.ResponseMessage);
                userEditorComponent = true;
                identityUsersListComponent = false;
                manageUserRolesComponent = false;
                ShowUserEditor(editUserViewModel.Id);
            }
            isBusy = false;
        }
    }
}
