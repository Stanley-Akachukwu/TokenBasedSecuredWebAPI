using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UCare.Models.AppSettingsModel;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/admin")]
    public class AdminController : ControllerBase
    {
      //  private readonly ILogger<AdminController> _logger;
      
        private IAdminRepository _adminService;
        public AdminController(IAdminRepository adminService)
        {
            _adminService = adminService;

        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _adminService.CreateRoleAsync(model);
                if (result.Succeeded)
                {
                 //   _logger.LogInformation($"Role  {model.RoleName} was Created successfully.");
                    return Created("", result);
                }
            }
            return BadRequest();
        }
        [Authorize(Policy = Policies.IsAdmin)]
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            List<IdentityRole> result = _adminService.ListRolesAsync();
            return Ok(result);
        }
        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            // Find the role by Role ID
            var role = await _adminService.GetRoleIdAsync(id);
            if (!role.Succeeded)
            {
                return BadRequest(role.ResponseMessage);
            }
            return  Ok(role);
        }

        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole(RoleViewModel model)
        {
            var role = await _adminService.UpdateRoleAsync(model);

           if (!role.Succeeded)
            {
                return BadRequest(role.ResponseMessage);
            }
            return Ok(role);
        }
        [HttpGet("GetUsersInRole/{id}")]
        public async Task<IActionResult> GetUsersInRole(string roleId)
        {
            var usersInRole = await _adminService.GetUsersInRoleAsync(roleId);
            if (!usersInRole[0].Succeeded)
            {
                return BadRequest(usersInRole[0].ResponseMessage);
            }

            return Ok(usersInRole);
        }
        [HttpPost("UpdateUsersInRole")]
        public async Task<IActionResult> UpdateUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var update = await _adminService.UpdateUsersInRoleAsync(model, roleId);
            if (!update[0].Succeeded)
            {
                return BadRequest(update[0].ResponseMessage);
            }
            return Ok(true);
        }
        [Authorize(Policy = Policies.IsAdmin)]
        [HttpGet("GetRolesForUser/{id}")]
        public async Task<IActionResult> GetRolesForUser(string id)
        {
            var response = await _adminService.GetRolesForUserAsync(id);
            if (!response.Succeeded)
            {
                return BadRequest(response.ResponseMessage);
            }
            return Ok(response);
        }
        [Authorize(Policy = Policies.IsAdmin)]
        [HttpPut("UpdateRolesForUser")]
        public async Task<IActionResult> UpdateRolesForUser(UserRolesViewModelUpdate modelUpdate)
        {
            var response = await _adminService.UpdateRolesForUserAsync(modelUpdate.EditedUserRoles, modelUpdate.UserId);
            if (!response.Succeeded)
            {
                return BadRequest(response.ResponseMessage);
            }
            return Ok(response);
        }
        [HttpGet("/api/GetUserClaims/{id}")]
        public async Task<IActionResult> GetUserClaims(string userId)
        {
            var model = await _adminService.GetUserClaimsAsync(userId);
            if (!model.Succeeded)
            {
                return BadRequest (model.ResponseMessage);
            }
            return Ok(model);
        }
        [HttpPost("UpdateUserClaims")]
        public async Task<IActionResult> UpdateUserClaims(UserClaimsViewModel model)
        {
            var result = await _adminService.UpdateUserClaimsAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.ResponseMessage);
            }
            return Ok(result);
        }
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _adminService.DeleteRoleAsync(id);
            if (!result.Succeeded)
            {
                return BadRequest(result.ResponseMessage);
            }
            return Ok(result);
        }
    }

}
