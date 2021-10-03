using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Controllers
{
   
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private IUserRepository _userService;
        private IMailRepository _mailService;
        private IConfiguration _configuration;
        public AuthController(IUserRepository userService, IMailRepository mailService, IConfiguration configuration)
        {
            _userService = userService;
            _mailService = mailService;
            _configuration = configuration;
        }

       
        // /api/auth/login
        [HttpPost("Login")]
        public async Task<ActionResult<UserManagerResponse>> LoginAsync([FromBody] LoginViewModel model)
        {
            var response = new UserManagerResponse();
            if (ModelState.IsValid)
            {
                 response = await _userService.LoginUserAsync(model);

                if (response.Succeeded)
                {
                   // await _mailService.SendEmailAsync(model.Email, "New login", "<h1>Hey!, new login to your account noticed</h1><p>New login to your account at " + DateTime.Now + "</p>");
                    return Ok(response);
                }
            }
            response.ResponseCode = StatusCodes.Status422UnprocessableEntity;
            response.ResponseMessage = "Invalid data supplied";
            return response;
        }

        // /api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }

            return BadRequest(result);
        }

        // api/auth/forgetpassword
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.Succeeded)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        // api/auth/resetpassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);

                if (result.Succeeded)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

    }
}
