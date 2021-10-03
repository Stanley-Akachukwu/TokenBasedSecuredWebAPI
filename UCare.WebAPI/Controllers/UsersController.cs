using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UCare.Models.AppSettingsModel;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/users")]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepo;
       // private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserManagerResponse>> Register([FromBody] RegisterViewModel model)
        {
            var response = new UserManagerResponse();
            if (ModelState.IsValid)
            {
                response = await _userRepo.RegisterUserAsync(model);
                if (response.Succeeded)
                {
                  //  _logger.LogInformation($"User with email  {model.Email} was Created successfully.");
                    return Ok(response);
                }
            }
            response.ResponseCode = StatusCodes.Status422UnprocessableEntity;
            response.ResponseMessage = "Invalid data supplied";
            return response;
        }
        [Authorize(Policy = Policies.IsAdminView)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            List<IdentityUser> users = _userRepo.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]        
        public async Task<IActionResult>GetUser(string id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (!user.Succeeded)
            {
               // _logger.LogInformation(user.ResponseMessage);
                return BadRequest(user.ResponseMessage);
            }
           // _logger.LogInformation($"Retrieved user with email {user.Email} successfully.");
            return Ok(user);
        }

        [HttpPut()]
        [Authorize(Policy = Policies.IsAdmin)]
        public async Task<IActionResult> UpdateUser(EditUserViewModel model)
        {
            var user = await _userRepo.UpdateUserAsync(model);
            if (!user.Succeeded)
            {
              //  _logger.LogInformation(user.ResponseMessage);
                return BadRequest(user.ResponseMessage);
            }
           // _logger.LogInformation($"Updated user with email {user.Email} successfully.");
            return Ok(user);
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.IsAdmin)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userRepo.DeleteUserAsync(id);
            if (!result.Succeeded)
            {
                //_logger.LogInformation(result.ResponseMessage);
                return BadRequest(result.ResponseMessage);
            }
         //   _logger.LogInformation($"Deleted user with email {id} successfully.");
            return Ok(result);
        }
    }
}
