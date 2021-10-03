using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ucare.WebUI.Services.Abstract;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;

namespace Ucare.WebUI.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            this._httpClient = httpClient;

        }
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {             
            return await _httpClient.PostJsonAsync<UserManagerResponse>("api/users/register", model);
        }
        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            return await _httpClient.PostJsonAsync<UserManagerResponse>("api/auth/login", model);
        }
        public Task<UserManagerResponse> ConfirmEmailAsync(string userId, string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task  DeleteUserAsync(string id, string accessToken)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                await _httpClient.DeleteAsync($"api/users/{id}");

            }
            catch (Exception )
            {
                return ;
            }
        }

        public Task<UserManagerResponse> ForgetPasswordAsync(string email, string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<EditUserViewModel> GetUserByIdAsync(string id,string accessToken)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var user= await _httpClient.GetJsonAsync<EditUserViewModel>($"api/users/{id}");
                return user;
            }
            catch (Exception)
            {
                return new EditUserViewModel { Succeeded = false, ResponseCode = StatusCodes.Status500InternalServerError };
            }
            
        }

        public async Task<IEnumerable<IdentityUser>> GetUsersAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var result = await _httpClient.GetJsonAsync<IdentityUser[]>("api/users/");
            return result;
        }

        public Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<EditUserViewModel> UpdateUserAsync(EditUserViewModel model, string accessToken)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var user = await _httpClient.PutJsonAsync<EditUserViewModel>("api/users", model);
                return user;
            }
            catch (Exception )
            {
                return new EditUserViewModel { Succeeded = false, ResponseCode = StatusCodes.Status500InternalServerError };
            }
        }

        public async Task<UserRolesViewModelResponse> GetRolesForUser(string id, string accessToken)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var result = await _httpClient.GetJsonAsync<UserRolesViewModelResponse>($"api/admin/GetRolesForUser/{id}");
                return result;
              
            }
            catch (Exception exp)
            {
                return new UserRolesViewModelResponse { Succeeded = false, ResponseCode = StatusCodes.Status500InternalServerError , ResponseMessage=exp.StackTrace};
            }
        }
        public async Task<UserRolesViewModelResponse> UpdateRolesForUser(string id, List<UserRolesViewModel> userRoles, string accessToken)
        {
            try
            {
                UserRolesViewModelUpdate modelUpdate = new UserRolesViewModelUpdate {
                EditedUserRoles = userRoles,
                UserId =id
                };
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var result = await _httpClient.PutJsonAsync<UserRolesViewModelResponse>($"api/admin/UpdateRolesForUser", modelUpdate);
                return result;

            }
            catch (Exception exp)
            {
                return new UserRolesViewModelResponse { Succeeded = false, ResponseCode = StatusCodes.Status500InternalServerError, ResponseMessage = exp.StackTrace };
            }
        }
    }
}
