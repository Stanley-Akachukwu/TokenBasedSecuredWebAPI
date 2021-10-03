using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UCare.Models.AppSettingsModel;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/Settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            this._settingsRepository = settingsRepository;
        }
        [HttpPost("Update-Aspnet-Roles-and-Claims")]
        public async Task<IActionResult> UpdateAspnetRolesAndClaimsTable()
        {
         var rolesWithClaimsUpdate = await  _settingsRepository.UpdateAspnetRolesTable();
            return Ok(rolesWithClaimsUpdate);
        }
    }
    
}
