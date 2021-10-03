using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Ucare.WebUI.Services.Abstract;
using Ucare.WebUI.Shared.Components;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Pages.Admin
{
    public class DisplayUserBase: ComponentBase
    {
        [Parameter]
        public IdentityUser registeredUser { get; set; }
        public bool isBusy = false;
        [Parameter]
        public EventCallback<string> OnRegisteredUserDeleted { get; set; }
        [Parameter]
        public EventCallback<string> OnShowUserEditor { get; set; }
        [Inject]
        public IUserService _userService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        protected ConfirmBase DeleteConfirmation { get; set; }
        protected void Delete_Click()
        {
            DeleteConfirmation.Show();
        }
        protected async void ShowUserEditor_Click()
        {
           await OnShowUserEditor.InvokeAsync(registeredUser.Id);
        }
        [Parameter]
        public LocalUserInfo localUserInfo { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Inject]
        private IAuthorizationService _authorizationService { get; set; }
        [Inject]
        public IToastService _toastService { get; set; }
        
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            var user = (await authenticationStateTask).User;
            isBusy = true;
            localUserInfo = await _localStorageService.GetItemAsync<LocalUserInfo>("User");
            if (deleteConfirmed)
            {
                await _userService.DeleteUserAsync(registeredUser.Id, localUserInfo.AccessToken);
                await OnRegisteredUserDeleted.InvokeAsync(registeredUser.Id);
                _toastService.ShowSuccess(registeredUser.UserName+" has been deleted!");
                _navigationManager.NavigateTo("/admin");
            }
        }
       
    }
}
