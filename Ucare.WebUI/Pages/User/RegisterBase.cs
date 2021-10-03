using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Ucare.WebUI.Models;
using Ucare.WebUI.Services.Abstract;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Pages.User
{
    public class RegisterBase: ComponentBase
    {
        [Parameter]
        public string Id { get; set; }
       public bool isBusy = false;
        public string message = string.Empty;
       public AlertMessageType messageType = AlertMessageType.Success;
        public RegisterViewModel registerViewModel { get; set; } = new RegisterViewModel();
        [Inject]
        public IUserService _userService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        [Inject]
        public IToastService _toastService { get; set; }
       
        protected async Task HandleValidSubmit()
        {
            isBusy = true;
            UserManagerResponse response = null;
            string errors = string.Empty;
            response = await _userService.RegisterUserAsync(registerViewModel);
            if (response.ResponseCode == StatusCodes.Status422UnprocessableEntity || response.ResponseCode == StatusCodes.Status409Conflict)
            {
                isBusy = false;
                foreach (var error in response.Errors)
                    errors += error + ". "; 
                errors += response.ResponseMessage;
                message = errors;
                messageType = AlertMessageType.Error;
                //ToastService.ShowError(errors);
            }
            if (response.ResponseCode == StatusCodes.Status201Created)
            {
                isBusy = false;
                _toastService.ShowSuccess(response.ResponseMessage);
                _navigationManager.NavigateTo("/login");
            }
        }
    }
}
