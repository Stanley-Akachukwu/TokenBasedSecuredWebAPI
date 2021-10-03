using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UCare.Models.AppSettingsModel;
using UCare.Models.UserViewModels;
using UCare.WebAPI.Data.Abstract;

namespace UCare.WebAPI.Data.Concrete
{
    public class SettingsRepository : ISettingsRepository
    {
        private RoleManager<IdentityRole> _roleManager;
        public SettingsRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<List<AspNetRoleClaims>> UpdateAspnetRolesTable()
        {
            List<AspNetRoleClaims> aspNetRoleWithClaims=  GetAspnetRolesAndClaimsStore();
            List<AspNetRoleClaims> aspNetRolesFromDB = GetAspnetRolesAndClaimsStore();
            var aspNetRoles = aspNetRoleWithClaims.Select(r => r.Role).ToList();
            foreach (var role in aspNetRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                    await _roleManager.CreateAsync(role);                 
            }
            return aspNetRoleWithClaims; // await GetAllUpdatedRolesWithClaims(aspNetRoleWithClaims);
        }

       
        public List<AspNetRoleClaims> GetAspnetRolesAndClaimsStore()
        {
            //Heads up!!!!!!!!!!!!!!!!!!!
            //Anytime you add item here, ensure you execute this method in the swagger to update the role and claim table.
            var aspNetRoleWithClaims = new List<AspNetRoleClaims>
            {
                new AspNetRoleClaims
                {
                    Role=new IdentityRole{ Name=Policies.Admin},Claims= new List<Claim>()
                    {
                        new Claim("Create Role", "true"),
                        new Claim("Delete Role", "true"),
                         new Claim("Edit Claim", "true")
                    }
                },
                 new AspNetRoleClaims
                {
                    Role=new IdentityRole{ Name=Policies.SuperAdmin},Claims= new List<Claim>()
                    {
                        new Claim("Create Role", "true"),
                        new Claim("Delete Role", "true"),
                         new Claim("Edit Claim", "true")
                    }
                },
                 new AspNetRoleClaims
                {
                    Role=new IdentityRole{ Name=Policies.User},Claims= new List<Claim>()
                },
            };
            return aspNetRoleWithClaims;
        }
        public List<Claim> GetClaimsStore()
        {
            List<AspNetRoleClaims> aspNetRoles = GetAspnetRolesAndClaimsStore();
            var allRoleClaims = new List<Claim>();
            foreach (var role in aspNetRoles)
                allRoleClaims.AddRange(role.Claims);


            var claims = new List<Claim>();
            foreach (var item in allRoleClaims)
                if (!claims.Contains(item))
                    claims.Add(item);
                return claims;
        }
    }
}