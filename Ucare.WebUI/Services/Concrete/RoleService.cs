using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ucare.WebUI.Services.Abstract;
using UCare.Models.UserViewModels;
using Microsoft.AspNetCore.Components;


namespace Ucare.WebUI.Services.Concrete
{
    public class RoleService : IRoleService
    {
        private readonly HttpClient _httpClient;
        public RoleService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<IdentityResult> CreateRole(RoleViewModel model, string accessToken)
        {
            return await _httpClient.PostJsonAsync<IdentityResult>("api/admin/createrole", model);
        }

        public async Task DeleteIdentityRole(string id, string accessToken)
        {
            await _httpClient.DeleteAsync($"api/admin/deleteRole/{id}"); 
        }

        public async Task<RoleViewModel> GetRole(string id, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await _httpClient.GetJsonAsync<RoleViewModel>($"api/admin/GetRoleById/{id}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<IdentityRole>> GetRoles(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            return await _httpClient.GetJsonAsync<List<IdentityRole>>("api/admin/GetAllRoles");
        }

        public async Task<RoleViewModel> UpdateRole(RoleViewModel model, string accessToken)
        {
            return await _httpClient.PutJsonAsync<RoleViewModel>("api/UpdateRole", model);
        }
    }
}
