using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ucare.WebUI.Models;
using Ucare.WebUI.Services.Abstract;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Pages.User
{
    public class LoginBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; }
        public bool isBusy = false;
        public string message = string.Empty;
        public AlertMessageType messageType = AlertMessageType.Success;
        public LoginViewModel loginViewModel { get; set; } =
           new LoginViewModel();
        [Inject]
        public IUserService _userService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }
        [Inject]
        public IToastService _toastService { get; set; }
        [Inject]
        public ILocalStorageService _localStorageService { get; set; }
        [Inject]
        public AuthenticationStateProvider _authenticationStateProvider { get; set; }

        protected async Task HandleLoginSubmit()
        {
            isBusy = true;
            UserManagerResponse response = null;
            string errors = string.Empty;
            response = await _userService.LoginUserAsync(loginViewModel);
            if (response.ResponseCode == StatusCodes.Status422UnprocessableEntity || response.ResponseCode == StatusCodes.Status409Conflict)
            {
                isBusy = false;
                foreach (var error in response.Errors)
                    errors += error+". ";
                errors += response.ResponseMessage;
                message = errors;
                messageType = AlertMessageType.Error;
                // _toastService.ShowError(errors);
            }
            if (response.Succeeded == true)
            {
                isBusy = false;
               var userInfo= GetUserClaims(response.Token);
                await _localStorageService.SetItemAsync("User",userInfo);
                await _authenticationStateProvider.GetAuthenticationStateAsync();
                _navigationManager.NavigateTo("/");
            }
        }

        protected LocalUserInfo GetUserClaims(string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            return new LocalUserInfo()
            {
                AccessToken = token,
                Email = tokenS.Claims.First(claim => claim.Type == "Email").Value,
                FirstName = tokenS.Claims.First(claim => claim.Type == "FirstName").Value,
                LastName = tokenS.Claims.First(claim => claim.Type == "LastName").Value,
                Id = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
            };
        }
        
    }
}

