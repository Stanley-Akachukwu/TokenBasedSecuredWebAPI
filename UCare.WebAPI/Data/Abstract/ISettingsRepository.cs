using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UCare.Models.AppSettingsModel;

namespace UCare.WebAPI.Data.Abstract
{
   public interface ISettingsRepository
    {
        Task<List<AspNetRoleClaims>> UpdateAspnetRolesTable();
        List<Claim> GetClaimsStore();
    }
}
