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
    public class RoleBase: ComponentBase
    {
        public bool isBusy = false;
        public string message = string.Empty;
        public bool showUserEditorPage = false;
        public AlertMessageType messageType = AlertMessageType.Success;
        public IdentityRole identityRole { get; set; } = new IdentityRole();
        public RoleViewModel editRoleViewModel { get; set; } = new RoleViewModel();
        public IEnumerable<IdentityRole> identityRoles { get; set; }
        [Inject]
        public IRoleService _roleService { get; set; }
        public string pageHeaderText { get; set; }
        [Parameter]
        public LocalUserInfo localUserInfo { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject]
        private IAuthorizationService _authorizationService { get; set; }
        [Inject]
        public IToastService _toastService { get; set; }
        public List<string> users { get; set; }
        protected async Task RoleDeleted()
        {
            identityRoles = (await _roleService.GetRoles(localUserInfo.AccessToken)).ToList();
        }
        protected void CancelRoleEdit()
        {
            showUserEditorPage = false;
            isBusy = false;
        }
        protected override async Task OnInitializedAsync()
        {
            isBusy = true;
            var user = (await authenticationStateTask).User;
            localUserInfo = await _localStorageService.GetItemAsync<LocalUserInfo>("User");
            try
            {
                if (user.Identity.IsAuthenticated)
                {
                    identityRoles = (await _roleService.GetRoles(localUserInfo.AccessToken)).ToList();
                }
            }
            catch (Exception exp)
            {
                isBusy = false;
                if (exp.Message.Contains("403") || exp.Message.Contains("401"))
                {
                    _toastService.ShowWarning("Sorry, you do not have access to the requested resource!");
                    _navigationManager.NavigateTo("/admin");
                }
            }
        }
        protected async Task ShowRoleEditor(string id)
        {
            var user = (await authenticationStateTask).User;
            isBusy = false;
            localUserInfo = await _localStorageService.GetItemAsync<LocalUserInfo>("User");
            if ((await _authorizationService.AuthorizeAsync(user, Policies.IsAdmin)).Succeeded)
            {
                if (id != null)
                {
                    editRoleViewModel = await _roleService.GetRole(id, localUserInfo.AccessToken);
                    if(editRoleViewModel!=null)
                        users = editRoleViewModel.Users;

                    pageHeaderText = "Edit "+ editRoleViewModel.RoleName;
                }
            }
            showUserEditorPage = true;
        }
        protected async Task HandleRoleEditUpdate()
        {
            isBusy = true;
            UserManagerResponse response = null;
            string errors = string.Empty;
            response = await _roleService.UpdateRole(editRoleViewModel, localUserInfo.AccessToken);
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
                isBusy = false;
                _toastService.ShowSuccess(response.ResponseMessage);
                _navigationManager.NavigateTo("/admin");
            }
        }
    }
}
