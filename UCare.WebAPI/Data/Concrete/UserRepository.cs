using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UCare.Models.UserModels;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using UCare.Models.AppSettingsModel;

namespace UCare.WebAPI.Data.Concrete
{
    public class UserRepository : IUserRepository
    {

        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IMailRepository _mailService;
        public UserRepository(UserManager<IdentityUser> userManager, IConfiguration configuration, IMailRepository mailService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _roleManager = roleManager;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
                throw new NullReferenceException("Reigster Model is null");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    ResponseMessage = "Invalid data supplied",
                    Succeeded = false,
                    Errors = new List<string> { "Confirm password doesn't match the password" }
                };

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email.Substring(0, model.Email.IndexOf("@"))
            };
            try
            {
                var result = await _userManager.CreateAsync(identityUser, model.Password);

                if (result.Succeeded)
                {
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                    var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                    //string url = $"{_configuration["AppUrl"]}/api/users/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                    //await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", $"<h1>Welcome to Auth Demo</h1>" +
                    //    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

                    if (await _roleManager.RoleExistsAsync(Policies.User))
                        await _userManager.AddToRolesAsync(identityUser, new List<string> { Policies.User });
                    //if (model.UserRole == UserRoles.SuperAdmin)
                    //{
                    //    if (await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                    //    await _userManager.AddToRolesAsync(identityUser, new List<string> { UserRoles.SuperAdmin,UserRoles.Admin, UserRoles.User });
                    //}
                    //if (model.UserRole == UserRoles.User || String.IsNullOrWhiteSpace(model.UserRole))
                    //{
                    //    if (await _roleManager.RoleExistsAsync(UserRoles.User))
                    //        await _userManager.AddToRolesAsync(identityUser, new List<string> { UserRoles.User });
                    //}
                    return new UserManagerResponse
                    {
                        ResponseMessage = $"User with email  {model.Email} was Created successfully.",
                        Succeeded = true,
                        ResponseCode = StatusCodes.Status201Created
                    };
                }
                else
                {
                    return new UserManagerResponse
                    {
                        ResponseMessage = "User was not created",
                        Succeeded = false,
                        Errors = result.Errors.Select(e => e.Description),
                        ResponseCode = StatusCodes.Status500InternalServerError,
                };
                }
            }
            catch(Exception exp)
            {
                return new UserManagerResponse
                {
                    ResponseMessage = "User was not created ",
                    Succeeded = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Errors=new List<string> { "Server error"+ exp.Message }
                };
            }
        }
        public List<IdentityUser> _users = new List<IdentityUser>
        {
            new IdentityUser{Id="932uikolwoiewpp830303j3bb3",Email="emezu@gmail.com"}
        };
        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = _users.First(); //await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    ResponseMessage = $"Invalid Data",
                    Succeeded = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Errors = new List<string> {$"There is no user with the Email address {model.Email} " }
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return new UserManagerResponse
                {
                    ResponseMessage = "Invalid data!",
                    Succeeded = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Errors = new List<string> { "Invalid password!" }
                };
            //   User 1: email:  password:Nedum @y12345 User 2: email: emezu @gmail.com password:mezu @G12345

            
            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim> 
            {
                new Claim("Email", user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            // ---------------For Demo on dev---------------------
            string nedum = "nedum@yahoo.com";
            string mezu = "emezu@gmail.com";
            if (user.Email == nedum)
            {
                userClaims.Add(new Claim("FirstName", "Chinedum"));
                userClaims.Add(new Claim("LastName", "Akachukwu"));
                userClaims.Add(new Claim(ClaimTypes.Role, "User","false"));
                userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
                userClaims.Add(new Claim("View users", "View users"));
            }
            if (user.Email == mezu)
            {
                userClaims.Add(new Claim("FirstName", "Emezu"));
                userClaims.Add(new Claim("LastName", "Akachukwu"));
                userClaims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            // ---------------For Demo on dev---------------------

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:SecreteKey"]));
            //foreach (var userRole in userRoles)
            //{
            //    userClaims.Add(new Claim(ClaimTypes.Role, userRole));
            //}
            var token = new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Token =  tokenAsString,
                ResponseMessage = "Successful",
                Succeeded = true,
                ExpireDate = token.ValidTo,
                ResponseCode = StatusCodes.Status200OK
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    Succeeded = false,
                    ResponseMessage = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    ResponseMessage = "Email confirmed successfully!",
                    Succeeded = true,
                };

            return new UserManagerResponse
            {
                Succeeded = false,
                ResponseMessage = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    Succeeded = false,
                    ResponseMessage = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");

            return new UserManagerResponse
            {
                Succeeded = true,
                ResponseMessage = "Reset password URL has been sent to the email successfully!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    Succeeded = false,
                    ResponseMessage = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Succeeded = false,
                    ResponseMessage = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    ResponseMessage = "Password has been reset successfully!",
                    Succeeded = true,
                };

            return new UserManagerResponse
            {
                ResponseMessage = "Something went wrong",
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }


        public List<IdentityUser> GetUsersAsync()
        {
            try
            {
                return _userManager.Users.ToList();
            }
            catch (Exception)
            {

                return new List<IdentityUser>();
            }
        }

        public async Task<EditUserViewModel> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var model = new EditUserViewModel();
            if (user == null)
            {
                model.ResponseMessage = $"User with Id = {id} cannot be found";
                model.Succeeded = false;
                return model;
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles,
                Succeeded = true
            };

            return model;
        }

        public async Task<EditUserViewModel> UpdateUserAsync(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                model = new EditUserViewModel();
                model.ResponseMessage = $"User with Id = {model.Id} cannot be found";
                model.Succeeded = false;
                return model;
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await _userManager.UpdateAsync(user);

                var userClaims = await _userManager.GetClaimsAsync(user);
                var userRoles = await _userManager.GetRolesAsync(user);
                model = new EditUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                    Roles = userRoles,
                    Succeeded = true,
                    ResponseCode=StatusCodes.Status200OK
                };
                return model;
            }
        }
        public async Task<UserManagerResponse> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var reponse = new UserManagerResponse();
            if (user == null)
            {
                reponse.ResponseMessage = $"User with Id = {id} cannot be found";
                reponse.Succeeded = false;
                return reponse;
            }
            else
            {
                try
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        reponse.ResponseMessage = $"User with Id = {id} has been removed successfully";
                        reponse.Succeeded = false;
                        return reponse;
                    }
                }
                catch (DbUpdateException ex)
                {
                    reponse.ResponseMessage = $"{user.UserName} cannot be deleted.  " + ex.StackTrace;
                    reponse.Succeeded = false;
                }
                return reponse;
            }
        }
    }
}
