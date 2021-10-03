using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ucare.WebUI.Services.Abstract;
using Ucare.WebUI.Shared.Components;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Pages.Admin
{
    public class DisplayRoleBase : ComponentBase
    {
        [Parameter]
        public IdentityRole identityRole { get; set; } = new IdentityRole();
        [Parameter]
        public EventCallback<string> OnShowRoleEditor { get; set; }
        [Inject]
        public IRoleService _roleService { get; set; }
        [Parameter]
        public EventCallback<string> OnRoleDeleted { get; set; }
        public NavigationManager _navigationManager { get; set; }
        protected ConfirmBase DeleteConfirmation { get; set; }
        [Parameter]
        public LocalUserInfo localUserInfo { get; set; }
        protected void Delete_Click()
        {
            DeleteConfirmation.Show();
        }
        protected async void ShowRoleEditor_Click()
        {
            await OnShowRoleEditor.InvokeAsync(identityRole.Id);
        }
       
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                await _roleService.DeleteIdentityRole(identityRole.Id, localUserInfo.AccessToken);
                await OnRoleDeleted.InvokeAsync(identityRole.Id);
                _navigationManager.NavigateTo("/admin");
            }
        }
    }
}
